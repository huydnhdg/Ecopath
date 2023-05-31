using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPGiaitriviet.Utils
{
    public class SmsTemp
    {
        public static string HHTD_VI = "Hoạt huyết T-Đình";
        public static string HHTD_EN = "Hoat huyet T-Dinh";
        public static string KTCD_VI = "Khớp tọa chi đan";
        public static string KTCD_EN = "Khop toa chi dan";
        public static string SUPERMAN = "Superman";
        public static string VVG_VI = "Viên vai gáy";
        public static string VVG_EN = "Vien vai gay";
        public static string MESSAGE = "SP chinh hang do Cty TNHH Ecopath VN phan phoi: Bifina ho tro hieu qua Viem Dai trang, Roi Loan Tieu Hoa.100% sx va dong goi tai Nhat Ban. Hotline: 02473046969";
        public static string ACTIVE_NOTVALID(string chanel)
        {
            string mtReturn = "Ma san pham khong dung. Quy khach vui long kiem tra lai ma san pham hoac lien he: 0934479596. Tran trong.";
            //if ("WEB".Equals(chanel))
            //{
            //    mtReturn = "Mã thẻ không tồn tại. Xin vui lòng thử lại. Liên hệ 1800.1716 để được hỗ trợ";
            //}
            return mtReturn;
        }

        public static string ACTIVE(string s1, string s2, string category)
        {
            string mtReturn = string.Format("SP chinh hang do Cty TNHH Ecopath VN phan phoi, QK da tich duoc {0} diem. Hay tich them {1} hop nua de nhan 1 hop BIFINA R20. LH 0934479596 de duoc huong dan.", s1, s2);
            if ("NATTO".Equals(category))
            {
                mtReturn = string.Format("Cam on Quy khach da mua dung SP chinh hang do Cty Ecopath VN phan phoi, Quy khach duoc {0} diem. Hay tich them {1} diem nua de nhan duoc qua tang.", s1, s2);
            }
            return mtReturn;

        }

        public static string ACTIVE_GIFT(string category)
        {
            string mtReturn = "Chuc mung ban da doi 1 hop BIFINA R20 thanh cong. Ban nen dung du lieu trinh Bifina de tang cuong suc khoe. LH 02473046969 de duoc ho tro.";
            if ("NATTO".Equals(category))
            {
                mtReturn = "Chuc mung QK da nhan duoc qua tang dac biet tu Ecopath VN la 1 hop NATTOKINASE 15 goi tri gia 360.000d. LH 0934479596 de duoc ho tro va tu van mien phi.";
            }
            return mtReturn;

        }

        // MT Trả về với các sản phẩm không thuộc các sản phẩm tích điểm.
        public static string ACTIVE_GENUINE(string category)
        {
            string mtReturn = "SP chinh hang do Cty TNHH Ecopath VN phan phoi: Bifina ho tro hieu qua Viem Dai trang, Roi Loan Tieu Hoa.100% sx va dong goi tai Nhat Ban. Hotline: 02473046969";
            // string mtReturn = "SP chinh hang do Cty TNHH Ecopath VN phan phoi: Nattokinase Jintan 100% hang duoc pham 125 tuoi Morishita Jintan Nhat Ban dong goi va sx. Hotline: 02473046969";
            if ("TAOMATTROI".Equals(category))
            {
                mtReturn = "SP chinh hang do Cty TNHH Ecopath VN phan phoi: Tao Mat Troi do hang Earthrise cua My san xuat. 100% Made in USA. Hotline: 02473046969";
            }
            else if ("JINTAN".Equals(category))
            {
                mtReturn = "SP chinh hang do Cty TNHH Ecopath VN phan phoi: Nattokinase Jintan 100% hang duoc pham 125 tuoi Morishita Jintan Nhat Ban dong goi va sx. Hotline: 02473046969";
            }
            return mtReturn;

        }

        public static string OVER_QUOTA(string chanel)
        {
            string mtReturn = "Tich diem khong thanh cong, A/c da tich luy qua han muc cua chuong trinh, moi so dien thoai duoc tich toi da 36 hop trong vong 6 thang. Hotline 1800.1716.";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Tích điểm không thành công, Anh/Chị đã tích lũy quá hạn mức của chương trình, mỗi số điện thoại được tích tối đa 36 hộp trên mỗi sản phẩm trong vòng 6 tháng. Hotline 1800.1716.";
            }
            return mtReturn;
        }

        public static string OVER_QUOTA_IN_MONTH(string chanel)
        {
            string mtReturn = "Tich diem khong thanh cong, A/c da tich luy qua han muc cua chuong trinh, moi so dien thoai duoc tich toi da 6 hop trong 1 thang. Hotline 1800.1716";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Tích điểm không thành công, Anh/Chị đã tích lũy quá hạn mức của chương trình, mỗi số điện thoại được tích tối đa 6 hộp trên mỗi sản phẩm trong vòng 1 tháng. Hotline 1800.1716.";
            }
            return mtReturn;

        }

        public static string ACTIVE(string s1, string s2, string s3, int? s4, string chanel)
        {
            string mtReturn = string.Format("Chuc mung ban co {0} diem va duoc tang {2} hop {1}. Chung toi se lien he voi ban de gui qua tang trong thoi gian som nhat. Hotline 1800.1716.", s1, s2, s4);
            if ("WEB".Equals(chanel))
            {
                mtReturn = string.Format("Chúc mừng bạn có {0} điểm và được tặng {2} hộp {1}. Chúng tôi sẽ liên hệ với bạn để gửi quà tặng trong thời gian sớm nhất. Hotline 1800.1716.", s1, s3, s4); ;
            }
            return mtReturn;

        }
        public static string ACTIVATED(string datetime)
        {
            string mtReturn = string.Format("Ma cao cua san pham da duoc xac thuc vao ngay {0}. LH Hotline: 02473046969 khi can ho tro. Cam on QK.", datetime);

            return mtReturn;

        }

        public static string SUPERMAN_SUPPERSEND(string chanel)
        {
            string mtReturn = "Chuong trinh tich diem doi qua san pham SuperMan da tam dung tu ngay 16/10/2021. Hotline: 18001716";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Chương trình tích điểm đổi quà sản phẩm SuperMan đã tạm dừng từ ngày 16/10/2021. Liên hệ 1800.1716 để được hỗ trợ.";
            }
            return mtReturn;

        }

        public static string KTCD_SUPPERSEND(string chanel)
        {
            string mtReturn = "Chuong trinh tich diem doi qua san pham Khop toa chi dan da tam dung tu ngay 23/12/2021. Hotline: 18001716";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Chương trình tích điểm đổi quà sản phẩm Khớp tọa chi đan đã tạm dừng từ ngày 23/12/2021. Liên hệ 1800.1716 để được hỗ trợ.";
            }
            return mtReturn;

        }

        public static string HHTD_SUPPERSEND(string chanel)
        {
            string mtReturn = "Chuong trinh tich diem doi qua san pham Hoat huyet T-Dinh da tam dung tu ngay 01/01/2022. Hotline: 18001716";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Chương trình tích điểm đổi quà sản phẩm Hoạt huyết T-Đình đã tạm dừng từ ngày 01/01/2022. Liên hệ 1800.1716 để được hỗ trợ.";
            }
            return mtReturn;

        }

        public static string VVG_SUPPERSEND(string chanel)
        {
            string mtReturn = "Chuong trinh tich diem doi qua san pham Vien vai gay da tam dung tu ngay 01/01/2022. Hotline: 18001716";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Chương trình tích điểm đổi quà sản phẩm Viên vai gáy đã tạm dừng từ ngày 01/01/2022. Liên hệ 1800.1716 để được hỗ trợ.";
            }
            return mtReturn;

        }

        //
        public static string TRACUU_NOTVALID(string chanel)
        {
            string mtReturn = "SDT cua A/C chua tham gia CTKM, hay mua Hoat huyet T-dinh, Superman, Khop toa chi dan, Vien vai gay de nhan nhieu qua tang hap dan. Hotline 1800.1716.";
            if ("WEB".Equals(chanel))
            {
                mtReturn = "Số điện thoại của Anh/Chị chưa tham gia CTKM, hãy mua Hoạt Huyết T-Đình, Superman, Khớp tọa chi đan, Viên vai gáy để nhận nhiều quà tặng hấp dẫn. Hotline 1800.1716.";
            }
            return mtReturn;

        }

        public static string TRACUU_VALID(string s1, string s2, string s3, string s4, string chanel)
        {

            string mtReturn = string.Format("A/C da tich luy {0} hop Hoat huyet T-Dinh chinh hang, {1} hop Superman, {2} hop Khop toa chi dan, {3} hop Vien vai gay. Hotline 1800.1716. http://quatang.duocbanhat.vn", s1, s2, s3, s4);

            if ("WEB".Equals(chanel))
            {
                mtReturn = string.Format("Anh chị đã tích lũy {0} hộp Hoạt huyết T-Đình chính hãng, {1} hộp Superman, {2} hộp Khớp tọa chi đan, {3} hộp Viên vai gáy. Hotline 1800.1716.", s1, s2, s3, s4);
            }
            return mtReturn;

        }

        public static string SYNTAX_INVALID(string chanel)
        {

            string mtReturn = string.Format("Tin nhan cua Quy khach sai cu phap. Vui long soan: ECO (dau cach) (Ma Cao) gui 8077 hoac lien he 0934479596 de duoc ho tro. Tran trong.");

            return mtReturn;

        }

        public static string CHECK_POINT(string point, DateTime datetime)
        {

            string mtReturn = string.Format("So diem cua quy khach la {0} diem, hay tiep tuc tich diem den het ngay {1} de nhan phan qua gia tri tu Ecopath VN. LH Hotline: 0934479596 khi can ho tro.", point, datetime.ToString("dd/MM/yy"));

            return mtReturn;

        }


        public static string NOT_HAS_POINT()
        {

            string mtReturn = "Quy khach chua co diem. hay tiep tuc tich diem de nhan phan qua gia tri tu Ecopath VN. LH Hotline: 0934479596 khi can ho tro.";

            return mtReturn;

        }

        public static string NOT_JOIN()
        {

            string mtReturn = "Quy khach chua tham gia chuong trinh. Vui long kiem tra lai va lien he Hotline: 0934479596. Tran trong.";

            return mtReturn;

        }

    }
}