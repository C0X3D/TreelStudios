using DatabaseContext;
using DatabaseContext.DataLayer;
using Microsoft.AspNetCore.Mvc;
using Treel.DataModel.Authentication.Responses;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels;
using Treel.Games.DataLayer;
using Treel.SendEndpointRequests;
using TreelGamesDataModel;
using TreelStudios.SignalR.DataLayer.Interfaces;

namespace TreelStudios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController(_IDatabaseContextManager databaseContextManager,
        HttpClient httpClient,
        IHubsService hubsService) : ControllerBase
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IHubsService _hubsService = hubsService;

        [HttpPost("SpinGame")]
        public async Task<ActionResult<SpinResponseModel>> SpinGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {                
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            CustomerRequestSender sender = new();

            //register spin
            WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);

            TransferBalanceResponseModel withdrawBalanceResponseModel = await sender.SendPlayerBalanceWithdrawRequestAsync(new CustomerSettings().Customer, withdrawBalanceRequestModel);            

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/GenerateSpin/GenerateSpin", null);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        SpinLogicDataModel dataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SpinLogicDataModel>(content)!;

                        if(dataModel != null)
                        {
                            if (await databaseContextManager.SetUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", new SpinLogicDataModelMongo(spinRequestModel, dataModel)))
                            {
                                if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
                                {
                                    return Ok(ResponseStatusEnum.USER_BUSY);
                                }
                            }
                        }
                        else
                        {
                            _ = await sender.SendPlayerBalanceDepositRequestAsync(new CustomerSettings().Customer, new DepositBalanceRequestModel()
                            {
                                Amount = spinRequestModel.Amount,
                                Customer_Curency_Name = spinRequestModel.Customer_Curency_Name,
                                GameName = spinRequestModel.GameId,
                                PlayerId = spinRequestModel.PlayerId,
                                SessionToken = spinRequestModel.SessionToken,
                                Customer_Site_Name = spinRequestModel.Customer_Site_Name,
                                IsRefund = true,
                                TransactionId = withdrawBalanceRequestModel.TransactionId,
                            });
                        }
                    }
                    else
                    {
                        // Handle error
                    }
                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                    }
                    break;

                default:
                    //throw error stop spin
                    //signalr
                    break;

            }

            return Ok();
        }

        [HttpPost("BuyBonusGame")]
        public async Task<ActionResult<SpinResponseModel>> BuyBonusGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            CustomerRequestSender sender = new();
            spinRequestModel.Amount = spinRequestModel.Amount * 177;
            //register spin
            WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);

            TransferBalanceResponseModel withdrawBalanceResponseModel = await sender.SendPlayerBalanceWithdrawRequestAsync(new CustomerSettings().Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/GenerateSpin/GenerateNormalBonus", null);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        SpinLogicDataModel dataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SpinLogicDataModel>(content)!;

                        if (dataModel != null)
                        {
                            if (await databaseContextManager.SetUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", new SpinLogicDataModelMongo(spinRequestModel, dataModel)))
                            {
                                if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
                                {
                                    return Ok(ResponseStatusEnum.USER_BUSY);
                                }
                            }
                        }
                        else
                        {
                            _ = await sender.SendPlayerBalanceDepositRequestAsync(new CustomerSettings().Customer, new DepositBalanceRequestModel()
                            {
                                Amount = spinRequestModel.Amount,
                                Customer_Curency_Name = spinRequestModel.Customer_Curency_Name,
                                GameName = spinRequestModel.GameId,
                                PlayerId = spinRequestModel.PlayerId,
                                SessionToken = spinRequestModel.SessionToken,
                                Customer_Site_Name = spinRequestModel.Customer_Site_Name,
                                IsRefund = true,
                                TransactionId = withdrawBalanceRequestModel.TransactionId,
                            });
                        }
                    }
                    else
                    {
                        // Handle error
                    }
                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                    }
                    break;

                default:
                    //throw error stop spin
                    //signalr
                    break;

            }

            return Ok();
        }

        [HttpPost("BuySuperBonusGame")]
        public async Task<ActionResult<SpinResponseModel>> BuySuperBonusGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            CustomerRequestSender sender = new();
            spinRequestModel.Amount = spinRequestModel.Amount * 600;
            //register spin
            WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);

            TransferBalanceResponseModel withdrawBalanceResponseModel = await sender.SendPlayerBalanceWithdrawRequestAsync(new CustomerSettings().Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/GenerateSpin/GenerateSuperBonus", null);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        SpinLogicDataModel dataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SpinLogicDataModel>(content)!;

                        if (dataModel != null)
                        {
                            if (await databaseContextManager.SetUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", new SpinLogicDataModelMongo(spinRequestModel, dataModel)))
                            {
                                if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
                                {
                                    return Ok(ResponseStatusEnum.USER_BUSY);
                                }
                            }
                        }
                        else
                        {
                            _ = await sender.SendPlayerBalanceDepositRequestAsync(new CustomerSettings().Customer, new DepositBalanceRequestModel()
                            {
                                Amount = spinRequestModel.Amount,
                                Customer_Curency_Name = spinRequestModel.Customer_Curency_Name,
                                GameName = spinRequestModel.GameId,
                                PlayerId = spinRequestModel.PlayerId,
                                SessionToken = spinRequestModel.SessionToken,
                                Customer_Site_Name = spinRequestModel.Customer_Site_Name,
                                IsRefund = true,
                                TransactionId = withdrawBalanceRequestModel.TransactionId,
                            });
                        }
                    }
                    else
                    {
                        // Handle error
                    }
                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                    }
                    break;

                default:
                    //throw error stop spin
                    //signalr
                    break;

            }

            return Ok();
        }

        [HttpPost("BuyLeetBonusGame")]
        public async Task<ActionResult<SpinResponseModel>> BuyLeetBonusGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            CustomerRequestSender sender = new();
            spinRequestModel.Amount = spinRequestModel.Amount * 1337;
            //register spin
            WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);

            TransferBalanceResponseModel withdrawBalanceResponseModel = await sender.SendPlayerBalanceWithdrawRequestAsync(new CustomerSettings().Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    await _hubsService.SendBalanceRefreshAsync($"{withdrawBalanceRequestModel.PlayerId}_{withdrawBalanceRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/GenerateSpin/GenerateLeetBonus", null);
                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        SpinLogicDataModel dataModel = Newtonsoft.Json.JsonConvert.DeserializeObject<SpinLogicDataModel>(content)!;

                        if (dataModel != null)
                        {
                            if (await databaseContextManager.SetUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", new SpinLogicDataModelMongo(spinRequestModel, dataModel)))
                            {
                                if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
                                {
                                    return Ok(ResponseStatusEnum.USER_BUSY);
                                }
                            }
                        }
                        else
                        {
                            _ = await sender.SendPlayerBalanceDepositRequestAsync(new CustomerSettings().Customer, new DepositBalanceRequestModel()
                            {
                                Amount = spinRequestModel.Amount,
                                Customer_Curency_Name = spinRequestModel.Customer_Curency_Name,
                                GameName = spinRequestModel.GameId,
                                PlayerId = spinRequestModel.PlayerId,
                                SessionToken = spinRequestModel.SessionToken,
                                Customer_Site_Name = spinRequestModel.Customer_Site_Name,
                                IsRefund = true,
                                TransactionId = withdrawBalanceRequestModel.TransactionId,
                            });
                        }
                    }
                    else
                    {
                        // Handle error
                    }
                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                    }
                    break;

                default:
                    //throw error stop spin
                    //signalr
                    break;

            }

            return Ok();
        }
    }
}