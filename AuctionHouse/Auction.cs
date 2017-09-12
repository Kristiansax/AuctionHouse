using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuctionHouse
{
    public class Auction
    {
        public string name { get; set; }
        public double currentbid { get; set; }
        public string currenthighestbidder { get; set; }
        public bool newhighestbidder { get; set; }
        public Auction (string n, double cb, string chb)
        {
            this.name = n;
            this.currentbid = cb;
            this.currenthighestbidder = chb;
            this.newhighestbidder = false;
        }
    }
}
