using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace GosZakupkiBot
{
    public class Item
    {
        [DisplayName("Номер")]
        public int Number { get; set; }
        [DisplayName("Статус")]
        public string Status { get; set; }
        [DisplayName("Возм.Ставка")]
        public float NextBet { get; set; }
        [DisplayName("Рентабел.")]
        public string Rentabel => $"{NextBet - MinPrice} Р ({Math.Round((int)(NextBet - MinPrice) / NextBet * 100, 1)})";
        [DisplayName("Мин.Цена")]
        public float MinPrice { get; set; }
        [DisplayName("Цена")]
        public float ActualPrice { get; set; }

        
        [Browsable(false)]
        public TimeSpan Ostalos { get; set; }

        [DisplayName("Осталось")]
        public string OstalosString { get {
                if(Ostalos.Days == 0)
                {
                    return $"{Ostalos.Hours} ч :{Ostalos.Minutes} м :{Ostalos.Seconds} с";
                }
                else return $"{Ostalos.Days} д :{Ostalos.Hours} ч :{Ostalos.Minutes} м :{Ostalos.Seconds} с";
        } set { } }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }
        [DisplayName("CRM")]
        public string CRMLink { get; set; }

    }
}