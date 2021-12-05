/*
IDamageDealer.cs

Description: A simple interface that forces its implementer to have a damage number.
Author: Yu Long
Created: Wednesday, December 01 2021
Unity Version: 2020.3.22f1c1
Contact: long_yu@berkeley.edu
*/
namespace Reimirno 
{
    public interface IDamageDealer
    {
        public int GetRawDamage();
    }
}
