
using DatabaseContext.DataLayer;
using DatabaseContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Treel.DataModel.Enums;
using Treel.DataModel.TransactionModels;
using Treel.Games.DataLayer;
using Treel.SendEndpointRequests;
using TreelGamesDataModel;
using Treel.DataModel.Authentication.Responses;
using VolatilTreels.Api.SignalR.DataLayer.Intefaces;
using System.Text;
using Newtonsoft.Json;
using Treel.Games.CrashApi.DataLayer;
using System.Reflection;

namespace VolatilTreels.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IHubsService _hubsService;

        private readonly CustomerRequestSender ClientRequestSender;        
        private readonly _IDatabaseContextManager databaseContextManager;

        public TransactionsController(_IDatabaseContextManager databaseContextManager,
            HttpClient httpClient,
            IHubsService hubsService)
        {
            this.databaseContextManager = databaseContextManager;
            _httpClient = httpClient;
            _hubsService = hubsService;
            ClientRequestSender = new CustomerRequestSender();
        }

        [HttpPost("SpinGame")]
        public async Task<ActionResult<SpinResponseModel>> SpinGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            

            //register spin
            TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);

            TreelStudios_BalanceResponseModel withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceWithdrawRequestAsync(CustomerSettings.InstanceSettings.Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    await _hubsService.SendBalanceRefreshAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https+http://{spinRequestModel.GameId}/api/SpinGenerator/GenerateSpin", null);
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
                            _ = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
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
                        return Ok(ResponseStatusEnum.UNEXPECTED_ERROR);
                    }
                    break;

                case WithdrawDepositStatus.TRANSACTION_ALREADY_EXISTS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.TRANSACTION_ALREADY_EXISTS);
                        return Ok(ResponseStatusEnum.UNEXPECTED_ERROR);
                    }
                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultIdx))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultIdx, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                        return Ok(ResponseStatusEnum.INSUFICIENT_FUNDS);
                    }
                    break;

                default:
                    {
                        return Ok(ResponseStatusEnum.UNEXPECTED_ERROR);
                    }
                    break;

            }

            return Ok();
        }

        [HttpPost("BuyBonusGame")]
        public async Task<ActionResult<SpinResponseModel>> BuyBonusGameAsync([FromBody] SpinRequestModel spinRequestModel)
        {
            if (spinRequestModel == null)
            {
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            //register spin
            TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);
            withdrawBalanceRequestModel.Amount = spinRequestModel.Amount * 177;

            TreelStudios_BalanceResponseModel withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceWithdrawRequestAsync(CustomerSettings.InstanceSettings.Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    await _hubsService.SendBalanceRefreshAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https+http://{spinRequestModel.GameId}/api/SpinGenerator/GenerateNormalBonus", null);
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
                            _ = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
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
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            //register spin
            TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);
            withdrawBalanceRequestModel.Amount = spinRequestModel.Amount * 600;

            TreelStudios_BalanceResponseModel withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceWithdrawRequestAsync(CustomerSettings.InstanceSettings.Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    await _hubsService.SendBalanceRefreshAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/SpinGenerator/GenerateSuperBonus", null);
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
                            _ = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
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
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(spinRequestModel.PlayerId) || string.IsNullOrEmpty(spinRequestModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));

            //IF BONUS HUNT
            if (await databaseContextManager.IsUserBusy($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}"))
            {
                return Ok(ResponseStatusEnum.USER_BUSY);
            }

            
            //register spin
            TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(spinRequestModel);
            withdrawBalanceRequestModel.Amount = spinRequestModel.Amount * 1337;

            TreelStudios_BalanceResponseModel withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceWithdrawRequestAsync(CustomerSettings.InstanceSettings.Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    await _hubsService.SendBalanceRefreshAsync($"{spinRequestModel.PlayerId}_{spinRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(resultId, WithdrawDepositStatus.SUCCESS);
                    }

                    await _hubsService.SendBalanceRefreshAsync($"{withdrawBalanceRequestModel.PlayerId}_{withdrawBalanceRequestModel.GameId}", withdrawBalanceResponseModel.Balance.ToString());
                    var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/SpinGenerator/GenerateLeetBonus", null);
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
                            _ = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="crashPlaceBetModel"></param>
        /// <returns>If BR&TRUE => refound succses</returns>
        /// <returns>If BR&FALSE => refound FAILED</returns>
        /// <returns></returns>
        [HttpPost("CrashGamePlaceBet")]
        public async Task<ActionResult<CrashBalanceResponseModel>> CrashGamePlaceBetAsync([FromBody] CrashPlaceBetModel crashPlaceBetModel)
        {
            if (crashPlaceBetModel == null)
            {
                return BadRequest(new TreelStudios_ErrorResponse()
                {
                    Status = ResponseStatusEnum.REQUEST_FAILED_OBJECT_NULL,
                });
            }

            if (string.IsNullOrEmpty(crashPlaceBetModel.PlayerId) || string.IsNullOrEmpty(crashPlaceBetModel.SessionToken))
                return BadRequest(new TreelStudios_ErrorResponse(ResponseStatusEnum.REQUEST_FAILED_OBJECT_PARAMS_NULL));


            //register spin
            TreelStudios_WithdrawBalanceRequestModel withdrawBalanceRequestModel = await databaseContextManager.StoreSpinWithdrawRequestAsync(crashPlaceBetModel);
            withdrawBalanceRequestModel.Amount = crashPlaceBetModel.Amount;

            TreelStudios_BalanceResponseModel withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceWithdrawRequestAsync(CustomerSettings.InstanceSettings.Customer, withdrawBalanceRequestModel);

            switch (withdrawBalanceResponseModel.Status)
            {
                case WithdrawDepositStatus.SUCCESS:
                    var placeBetModel = JsonConvert.SerializeObject(new CrashBetRequest() { UserId = withdrawBalanceRequestModel.SessionToken!, Amount = (double)withdrawBalanceRequestModel.Amount, UserName = crashPlaceBetModel.PlayerName });
                    HttpContent placeBetModelcontent = new StringContent(placeBetModel, Encoding.UTF8, "application/json");
                    var response = await _httpClient.PostAsync($"https://{crashPlaceBetModel.GameId}/api/GameLogic/place-bet", placeBetModelcontent);

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        CrashBetResponse dataModel = JsonConvert.DeserializeObject<CrashBetResponse>(content)!;

                        if (dataModel != null)
                        {
                            switch (dataModel.Success)
                            {
                                case CrashBetResponseStatus.Success:
                                    return Ok(new CrashBalanceResponseModel() { Status = WithdrawDepositStatus.SUCCESS, NewBalance = withdrawBalanceResponseModel.Balance });

                               
                                case CrashBetResponseStatus.Failed:
                                case CrashBetResponseStatus.DuplicateBet:
                                    //REFUND
                                    withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
                                    {
                                        Amount = crashPlaceBetModel.Amount,
                                        Customer_Curency_Name = crashPlaceBetModel.Customer_Curency_Name,
                                        GameName = crashPlaceBetModel.GameId,
                                        PlayerId = crashPlaceBetModel.PlayerId,
                                        SessionToken = crashPlaceBetModel.SessionToken,
                                        Customer_Site_Name = crashPlaceBetModel.Customer_Site_Name,
                                        IsRefund = true,
                                        TransactionId = withdrawBalanceRequestModel.TransactionId,
                                    });
                                    return BadRequest(new CrashBalanceResponseModel() { Status = withdrawBalanceResponseModel.Status == WithdrawDepositStatus.SUCCESS ? WithdrawDepositStatus.REFUND_SUCCESS : WithdrawDepositStatus.FAILED, NewBalance = withdrawBalanceResponseModel.Balance });

                            }
                        }
                        else
                        {
                            //refund
                            withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
                            {
                                Amount = crashPlaceBetModel.Amount,
                                Customer_Curency_Name = crashPlaceBetModel.Customer_Curency_Name,
                                GameName = crashPlaceBetModel.GameId,
                                PlayerId = crashPlaceBetModel.PlayerId,
                                SessionToken = crashPlaceBetModel.SessionToken,
                                Customer_Site_Name = crashPlaceBetModel.Customer_Site_Name,
                                IsRefund = true,
                                TransactionId = withdrawBalanceRequestModel.TransactionId,
                            });

                            return BadRequest(new CrashBalanceResponseModel() { Status = withdrawBalanceResponseModel.Status == WithdrawDepositStatus.SUCCESS ? WithdrawDepositStatus.REFUND_SUCCESS : WithdrawDepositStatus.FAILED, NewBalance = withdrawBalanceResponseModel.Balance });
                        }
                    }
                    else
                    {
                        //refund
                        withdrawBalanceResponseModel = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
                        {
                            Amount = crashPlaceBetModel.Amount,
                            Customer_Curency_Name = crashPlaceBetModel.Customer_Curency_Name,
                            GameName = crashPlaceBetModel.GameId,
                            PlayerId = crashPlaceBetModel.PlayerId,
                            SessionToken = crashPlaceBetModel.SessionToken,
                            Customer_Site_Name = crashPlaceBetModel.Customer_Site_Name,
                            IsRefund = true,
                            TransactionId = withdrawBalanceRequestModel.TransactionId,
                        });

                        return BadRequest(new CrashBalanceResponseModel() { Status = withdrawBalanceResponseModel.Status == WithdrawDepositStatus.SUCCESS ? WithdrawDepositStatus.REFUND_SUCCESS : WithdrawDepositStatus.FAILED, NewBalance = withdrawBalanceResponseModel.Balance });

                    }


                    break;

                case WithdrawDepositStatus.INSUFICIENT_FUNDS:
                    if (Guid.TryParse(withdrawBalanceRequestModel.TransactionId, out var __resultId))
                    {
                        await databaseContextManager.UpdateSpinWithdrawRequestAsync(__resultId, WithdrawDepositStatus.INSUFICIENT_FUNDS);
                        return Ok(new CrashBalanceResponseModel() { Status = WithdrawDepositStatus.INSUFICIENT_FUNDS, NewBalance = withdrawBalanceResponseModel.Balance });
                    }
                    break;

                default:
                    //throw error stop spin
                    //signalr
                    break;
            }

            return Ok();
        }

        [HttpPost("CrashGameCashout")]
        public async Task<ActionResult<CrashBalanceResponseModel>> CrashGameCashoutBetAsync([FromBody] CrashPlaceBetModel spinRequestModel)
        {
            var xbody = JsonConvert.SerializeObject(new CrashCashOutRequest() { SessionToken = spinRequestModel.SessionToken, GameId = spinRequestModel.GameId, UserName = spinRequestModel.PlayerName });
            HttpContent xcontent = new StringContent(xbody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"https://{spinRequestModel.GameId}/api/GameLogic/cash-out", xcontent);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                CrashCashOutResponse dataModel = JsonConvert.DeserializeObject<CrashCashOutResponse>(content)!;

                if (dataModel.Multiplier > 0)
                {
                    var depositBalanceHelp = await databaseContextManager.StoreSpinDepositRequestAsync(spinRequestModel);
                    var depositresponse = await ClientRequestSender.SendPlayerBalanceDepositRequestAsync(CustomerSettings.InstanceSettings.Customer, new TreelStudios_DepositBalanceRequestModel()
                    {
                        Amount = (decimal)dataModel.Multiplier,
                        Customer_Curency_Name = CustomerSettings.InstanceSettings.Customer.Customer_Curency_Name,
                        GameName = spinRequestModel.GameId,
                        PlayerId = spinRequestModel.PlayerId,
                        SessionToken = spinRequestModel.SessionToken,
                        Customer_Site_Name = spinRequestModel.Customer_Site_Name,
                        IsRefund = false,
                        TransactionId = depositBalanceHelp.TransactionId,
                    });

                    if (depositresponse.Status == WithdrawDepositStatus.SUCCESS)
                    {
                        return Ok(new CrashBalanceResponseModel() { Status = WithdrawDepositStatus.SUCCESS, NewBalance = depositresponse.Balance });
                    }
                    else
                    {
                        return BadRequest(new { Status = WithdrawDepositStatus.FAILED, error = depositresponse });
                    }
                }
            }
            else
            {
                return BadRequest(new { error = await response.Content.ReadAsStringAsync() });
            }

            throw new Exception("FAILED SOMETHING");
        }

        [HttpGet("CrashGameStart")]
        public async Task<bool> CrashGameStartAsync(string crashGame)
        {
            var response = await _httpClient.GetAsync($"http+https://{crashGame}/api/GameLogic/StartServer");
            return response.IsSuccessStatusCode;
        }
    }
}