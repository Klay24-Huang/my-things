using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace All._735._Asteroid_Collision
{
    public class Solution
    {
        public int[] AsteroidCollision(int[] asteroids)
        {
            var stack = new Stack<int>();
            foreach (var asteroid in asteroids)
            {
                if (stack.Count == 0 || asteroid > 0)
                {
                    stack.Push(asteroid);
                    continue;
                }

                if (asteroid < 0)
                {
                    var peek = stack.Peek();
                    while (true)
                    {
                        if (peek + asteroid > 0)
                        {
                            break;
                        }

                        if (peek + asteroid == 0)
                        {
                            stack.Pop();
                            break;
                        }

                        if (peek < 0)
                        {
                            stack.Push(asteroid);
                            break;
                        }

                        if (peek + asteroid < 0)
                        {
                            stack.Pop();
                            var hasItem = stack.TryPeek(out peek);
                            if (!hasItem || peek < 0)
                            {
                                stack.Push(asteroid);
                                break;
                            }
                        }
                    }
                }
            }
            return stack.Reverse().ToArray();
        }
    }
}
