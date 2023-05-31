using GPGiaitriviet.Models;
using GPGiaitriviet.Utils;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace GP_Giaitriviet.API
{
    [RoutePrefix("api/sms")]
    public class SmsProviderController : ApiController
    {
        Logger log = LogManager.GetCurrentClassLogger();
        GiaitrivietEntities db = new GiaitrivietEntities();
        // private readonly int NUMBER_OVER_QUOTA = 36;
        // private readonly int NUMBER_OVER_QUOTA_IN_MONTH = 4;

        [Route("receive")]
        [HttpGet]
        public HttpResponseMessage Receive(string Command_Code, string User_ID, string Service_ID, string Request_ID, string Message)
        {
            // Lưu log file
            log.Info(string.Format("[MO] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, Message));
            User_ID = MyControl.formatUserId(User_ID, 0);
            // Lưu log vào DB
            var LogMO = new LogMO()
            {
                Command_Code = Command_Code,
                Phone = User_ID,
                Service_ID = Service_ID,
                Request_ID = Request_ID,
                Message = Message,
                Createdate = DateTime.Now
            };
            db.LogMOes.Add(LogMO);
            db.SaveChanges();
            long id = LogMO.ID;
            // Kết thúc ghi log vào DB

            string mt_trakhachhang = "";
            string category = "TC";
            string chanel = "SMS";
            int status = 0; // 0 - thành công, 1- thất bại, 2 - tra cứu, 3 - sai cú pháp, 4 - trùng mã thẻ, 5 - Quá hạn mức trong 6 tháng, 6- Quá hạn mức trong 1 tháng
            if (Request_ID.ToUpper().StartsWith("WEB"))
            {
                chanel = "WEB";
            }
            try
            {
                string[] words = Message.ToUpper().Split(' ');
                if (words.Length < 2)
                {
                    mt_trakhachhang = SmsTemp.SYNTAX_INVALID(chanel);
                    category = "SYNTAX_INVALID";
                    status = 3;
                }
                else if (words[1] == "TC")
                {
                    if (words.Length == 2)
                    {
                        mt_trakhachhang = SmsTemp.SYNTAX_INVALID(chanel);
                        category = "SYNTAX_INVALID";
                        status = 3;
                    }
                    else
                    {
                        String Code = words[2];
                        var codeGP = db.CodeGPs.Where(a => a.Code == Code.Trim()).SingleOrDefault();

                        if (codeGP == null || codeGP.Status == 0)
                        {
                            mt_trakhachhang = SmsTemp.ACTIVE_NOTVALID(chanel);
                            status = 2;
                            category = "ERROR_CODE";
                        }
                        else
                        {
                            mt_trakhachhang = SmsTemp.ACTIVATED(codeGP.Activedate.GetValueOrDefault().ToString("dd/MM/yyyy"));
                            status = 2;
                            category = "ACTIVED";
                        }
                    }
                }
                else if (words[1] == "DIEM")
                {
                    if (words.Length == 2)
                    {
                        mt_trakhachhang = SmsTemp.SYNTAX_INVALID(chanel);
                        category = "SYNTAX_INVALID";
                        status = 3;
                    }
                    else
                    {
                        String Code = words[2];
                        var customer = db.Customers.Where(x => x.Phone == User_ID).FirstOrDefault();

                        if (customer == null)
                        {
                            mt_trakhachhang = SmsTemp.NOT_JOIN();
                            status = 2;
                            category = "ERROR_CODE";
                        }
                        else
                        {
                             if ("NATTO".Equals(Code))
                            {
                                if (customer.NATTO == 0)
                                {
                                    mt_trakhachhang = SmsTemp.NOT_HAS_POINT();
                                }
                                else
                                {
                                    mt_trakhachhang = SmsTemp.CHECK_POINT(customer.NATTO.ToString(), customer.LastNatto.GetValueOrDefault().AddMonths(4));
                                }
                            } 
                            //else if ("X30".Equals(Code))
                            //{
                            //    if (customer.X30 == 0)
                            //    {
                            //        mt_trakhachhang = SmsTemp.NOT_HAS_POINT();
                            //    }
                            //    else
                            //    {
                            //        mt_trakhachhang = SmsTemp.CHECK_POINT(customer.X30.ToString(), customer.LastX30.GetValueOrDefault().AddMonths(4));
                            //    }
                            //}                            
                            else
                            {
                                //mt_trakhachhang = SmsTemp.SYNTAX_INVALID(chanel);
                                mt_trakhachhang = SmsTemp.MESSAGE;
                                category = "SYNTAX_INVALID";
                                status = 3;
                            }
                        }
                    }
                }
                else
                {
                    String Code = words[1];
                    log.Info(string.Format("Phone = {0}, Code = {1}", User_ID, Code));
                    var codeGP = db.CodeGPs.Where(a => a.Code == Code.Trim()).SingleOrDefault();

                    if (codeGP == null)
                    {
                        mt_trakhachhang = SmsTemp.ACTIVE_NOTVALID(chanel);
                        status = 1;
                        category = "ERROR_CODE";
                    }
                    else
                    {
                        category = codeGP.Category.ToUpper();
                        // Đã kích hoạt bảo hành trước đó
                        if (codeGP.Status == 1)
                        {
                            mt_trakhachhang = SmsTemp.ACTIVATED(codeGP.Activedate.GetValueOrDefault().ToString("dd/MM/yyyy"));
                            status = 4;
                        }
                        else
                        {
                            var customer1 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                            if (customer1 == null)
                            {
                                // Tích lũy cho khách hàng
                                codeGP.Status = 1;
                                codeGP.Phone = User_ID;
                                codeGP.Activedate = DateTime.Now;
                                // Cập nhật thông tin khách hàng
                                db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges(); // Thêm ngày 31/07/2021
                                //Insert vào bảng Customer
                                Customer newCustomer = new Customer();
                                switch (category)
                                {
                                    case "X30":
                                        newCustomer = new Customer()
                                        {
                                            Phone = User_ID,
                                            X30 = 1,
                                            NATTO = 0,
                                            JINTAN = 0,
                                            TAOMATTROI = 0,
                                            OTHER = 0,
                                            Createdate = DateTime.Now,
                                            LastX30 = DateTime.Now
                                        };
                                        //mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                        mt_trakhachhang = SmsTemp.MESSAGE;
                                        break;
                                    case "NATTO":
                                        newCustomer = new Customer()
                                        {
                                            Phone = User_ID,
                                            X30 = 0,
                                            NATTO = 1,
                                            JINTAN = 0,
                                            TAOMATTROI = 0,
                                            OTHER = 0,
                                            Createdate = DateTime.Now,
                                            LastX30 = DateTime.Now
                                        };
                                        mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                        break;
                                    case "JINTAN":
                                        newCustomer = new Customer()
                                        {
                                            Phone = User_ID,
                                            X30 = 0,
                                            NATTO = 0,
                                            JINTAN = 1,
                                            TAOMATTROI = 0,
                                            OTHER = 0,
                                            Createdate = DateTime.Now,
                                            LastX30 = DateTime.Now
                                        };
                                        mt_trakhachhang = SmsTemp.ACTIVE_GENUINE("JINTAN");
                                        break;
                                    case "TAOMATTROI":
                                        newCustomer = new Customer()
                                        {
                                            Phone = User_ID,
                                            X30 = 0,
                                            NATTO = 0,
                                            JINTAN = 0,
                                            TAOMATTROI = 1,
                                            OTHER = 0,
                                            Createdate = DateTime.Now,
                                            LastX30 = DateTime.Now
                                        };
                                        mt_trakhachhang = SmsTemp.ACTIVE_GENUINE("TAOMATTROI");
                                        break;
                                    default:
                                        newCustomer = new Customer()
                                        {
                                            Phone = User_ID,
                                            X30 = 0,
                                            NATTO = 0,
                                            OTHER = 1,
                                            JINTAN = 0,
                                            TAOMATTROI = 0,
                                            Createdate = DateTime.Now,
                                            LastX30 = DateTime.Now
                                        };
                                        mt_trakhachhang = SmsTemp.ACTIVE_GENUINE("OTHER");
                                        break;
                                }
                                db.Customers.Add(newCustomer);
                                db.SaveChanges();
                            }
                            else
                            {
                                if (/*category.Equals("X30") || */category.Equals("NATTO"))
                                {

                                    // Kiểm tra thời gian tích lũy lần cuối, nếu quá 4 tháng thì reset
                                    // DateTime? StartTimeActive = DateTime.Now;
                                    DateTime? StartTimeActive = null;
                                    switch (category)
                                    {
                                        //case "X30":
                                        //    StartTimeActive = customer1.LastX30;
                                        //    break;
                                        case "NATTO":
                                            StartTimeActive = customer1.LastNatto;
                                            break;
                                    }

                                    // Xử lý cho các thông tin thuê bao chưa có thời gian Active lần đầu.
                                    if (StartTimeActive == null)
                                    {
                                        log.Info(string.Format("Thuê bao {0} đang có Start Time Active là NULL", User_ID));
                                        var LastTime = db.CodeGPs.Where(x => x.Phone == User_ID && x.Category == category && x.Status == 1).OrderByDescending(x => x.Activedate).FirstOrDefault();
                                        if (LastTime != null && LastTime.Activedate != null)
                                        {
                                            StartTimeActive = LastTime.Activedate;
                                            switch (category)
                                            {
                                                //case "X30":
                                                //    customer1.LastX30 = LastTime.Activedate;
                                                //    break;
                                                case "NATTO":
                                                    customer1.LastNatto = LastTime.Activedate;
                                                    break;
                                            }
                                            log.Info(string.Format("Cập nhật thời gian kích hoạt nhóm sản phẩm {0} cho thuê bao {1} với thời gian là {2}", category, User_ID, StartTimeActive));
                                            db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }

                                    // Tích lũy lần đầu
                                    if (StartTimeActive == null)
                                    {
                                        log.Info(string.Format("Thuê bao {0} kích hoạt sản phẩm lần đầu", User_ID));
                                        codeGP.Status = 1;
                                        codeGP.Phone = User_ID;
                                        codeGP.Activedate = DateTime.Now;
                                        // Cập nhật thông tin khách hàng
                                        db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        // Reset lại điểm của khách hàng
                                        switch (category)
                                        {
                                            //case "X30":
                                            //    customer1.X30 = 1;
                                            //    customer1.LastX30 = codeGP.Activedate;
                                            //    mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                            //    break;
                                            case "NATTO":
                                                customer1.NATTO = 1;
                                                customer1.LastNatto = codeGP.Activedate;
                                                mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                                break;
                                        }
                                        db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else if (StartTimeActive.GetValueOrDefault().AddMonths(4) >= DateTime.Now)
                                    {
                                        // Tích lũy cho khách hàng.
                                        codeGP.Status = 1;
                                        codeGP.Phone = User_ID;
                                        codeGP.Activedate = DateTime.Now;
                                        // Cập nhật thông tin khách hàng
                                        db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges(); // Thêm ngày 31/07/2021                                      
                                        int? NumberOfCode = 0;
                                        var customer2 = db.Customers.Where(a => a.Phone == User_ID).SingleOrDefault();
                                        // log.Info(string.Format("Thuê bao {0} Có {1} HHTD, {2} KTCD, {3} Superman, {4} VVG", User_ID, customer2.HHTD, customer2.KTCD, customer2.SUPERMAN, customer2.VVG));
                                        switch (category)
                                        {
                                            //case "X30":
                                            //    customer2.X30++;
                                            //    NumberOfCode = customer2.X30;
                                            //    customer2.LastX30 = codeGP.Activedate;
                                            //    if (NumberOfCode == 3)
                                            //    {
                                            //        customer2.X30 = 0;
                                            //    }
                                            //    break;
                                            case "NATTO":
                                                customer2.NATTO++;
                                                NumberOfCode = customer2.NATTO;
                                                customer2.LastNatto = codeGP.Activedate;
                                                if (NumberOfCode == 3)
                                                {
                                                    customer2.NATTO = 0;
                                                }
                                                break;
                                        }
                                        db.Entry(customer2).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();

                                        if (NumberOfCode < 3)
                                        {
                                            int? SoHopConLai = 3 - NumberOfCode;
                                            mt_trakhachhang = SmsTemp.ACTIVE(NumberOfCode.ToString(), SoHopConLai.ToString(), category);
                                        }
                                        else if (NumberOfCode == 3)
                                        {
                                            var dem = db.GiftExchanges.Where(a => a.Phone == User_ID && a.Product == category);
                                            int demsolan = dem.Count();
                                            demsolan++;

                                            // Insert vào bảng GiftExchange
                                            GiftExchange giftExchange = new GiftExchange()
                                            {
                                                Phone = User_ID,
                                                Product = category,
                                                Createdate = DateTime.Now,
                                                Status = 0,
                                                Count = demsolan
                                            };
                                            db.GiftExchanges.Add(giftExchange);
                                            db.SaveChanges();
                                            mt_trakhachhang = SmsTemp.ACTIVE_GIFT(category);
                                        }
                                    }
                                    else // Quá 4 tháng từ ngày kích hoạt sản phẩm đầu tiên
                                    {
                                        log.Info(string.Format("Thuê bao {0} quá 4 tháng kể từ ngày kích hoạt cuối cùng", User_ID));
                                        codeGP.Status = 1;
                                        codeGP.Phone = User_ID;
                                        codeGP.Activedate = DateTime.Now;
                                        // Cập nhật thông tin khách hàng
                                        db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                        // Reset lại điểm của khách hàng
                                        switch (category)
                                        {
                                            //case "X30":
                                            //    customer1.X30 = 1;
                                            //    customer1.LastX30 = DateTime.Now;
                                            //    mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                            //    break;
                                            case "NATTO":
                                                customer1.NATTO = 1;
                                                customer1.LastNatto = DateTime.Now;
                                                mt_trakhachhang = SmsTemp.ACTIVE("1", "2", category);
                                                break;
                                        }
                                        db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    codeGP.Status = 1;
                                    codeGP.Phone = User_ID;
                                    codeGP.Activedate = DateTime.Now;
                                    // Cập nhật thông tin khách hàng
                                    db.Entry(codeGP).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                    // Trả thông tin về cho khách hàng kích hoạt sản phẩm chính hãng
                                    switch (category)
                                    {
                                        case "X30":
                                            //thêm vào ngày 25/11/2022
                                            customer1.X30++;
                                            customer1.LastX30 = codeGP.Activedate;
                                            break;
                                        case "JINTAN":// Sản phẩm JINTAN cũ
                                            customer1.JINTAN++;
                                            customer1.LastJinTan = codeGP.Activedate;
                                            break;
                                        case "TAOMATTROI":
                                            customer1.TAOMATTROI++;
                                            customer1.LastTaoMatTroi = codeGP.Activedate;
                                            break;
                                        default:
                                            customer1.OTHER++;
                                            customer1.LastOther = codeGP.Activedate;
                                            break;
                                    }
                                    mt_trakhachhang = SmsTemp.ACTIVE_GENUINE(category);
                                    db.Entry(customer1).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            var response = new HttpResponseMessage();
            // Lưu log xử lý vào bảng MT
            var LogMT = new LogMT()
            {
                MO_ID = id,
                Phone = User_ID,
                Createdate = DateTime.Now,
                Message = mt_trakhachhang,
                Product = category,
                Chanel = chanel,
                Status = status
            };
            db.LogMTs.Add(LogMT);
            db.SaveChanges();
            // Kết thúc lưu log MT
            var result = new Result()
            {
                status = "0",
                message = mt_trakhachhang,
                phone1 = User_ID
            };
            log.Info(string.Format("[MT] @Command_Code= {0} @User_ID= {1} @Service_ID= {2} @Request_ID= {3} @Message= {4}", Command_Code, User_ID, Service_ID, Request_ID, mt_trakhachhang));
            return ResponseMessage(result);
        }
        private HttpResponseMessage ResponseMessage(Result result)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(JsonConvert.SerializeObject(result), Encoding.UTF8, "application/json");
            return response;
        }

    }
}