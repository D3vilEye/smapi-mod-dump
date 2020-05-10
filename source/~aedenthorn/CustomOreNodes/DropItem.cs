﻿namespace CustomOreNodes
{
    public class DropItem
    {
        public string v;
        public string itemIdOrName;
        public float dropChance;
        public int minAmount;
        public int maxAmount;
        public int luckyAmount;
        public int minerAmount;

        public DropItem(string itemInfo)
        {
            int i = 0;
            string[] infos = itemInfo.Split(';');
            this.itemIdOrName = infos[i++];
            this.dropChance = float.Parse(infos[i++]);
            this.minAmount = int.Parse(infos[i].Split(',')[0]);
            this.maxAmount = int.Parse(infos[i++].Split(',')[1]);
            this.luckyAmount = int.Parse(infos[i++]);
            this.minerAmount = int.Parse(infos[i++]);

        }
    }
}