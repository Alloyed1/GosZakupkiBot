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
        public string Rentabel => $"{NextBet - MinPrice} Р ({Math.Round((int)(NextBet - MinPrice)/NextBet*100, 1)})";
        [DisplayName("Мин.Цена")]
        public float MinPrice { get; set; }
        [DisplayName("Цена")]
        public float ActualPrice { get; set; }

        [DisplayName("Осталось")]
        public TimeSpan Ostalos { get; set; }

        [DisplayName("Комментарий")]
        public string Comment { get; set; }
        [DisplayName("CRM")]
        public string CRMLink { get; set; }

    }
}