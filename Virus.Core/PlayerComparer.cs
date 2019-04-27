using System;
using System.Collections.Generic;
using System.Text;

namespace Virus.Core
{
    public class PlayerComparer : IComparer<Player>
    {

        public int Compare(Player x, Player y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (y == null)
                {
                    return -1;
                }
                else
                {
                    int i, j;

                    i = x.HealthyOrgans;
                    j = y.HealthyOrgans;

                    if (i == j)
                    {
                        i = x.Body.Points;
                        j = y.Body.Points;

                        if (i == j)
                        {
                            return 0;
                        }
                        else
                        {
                            if (i > j)
                            {
                                return -1;
                            }
                            else
                            {
                                return 1;
                            }
                        }
                    }
                    else
                    {
                        if (i > j)
                        {
                            return -1;
                        }
                        else
                        {
                            return 1;
                        }
                    }
                }
            }
        }
    }
}
