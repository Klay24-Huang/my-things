import unittest
import solution as s

class Arg:
    def __init__(self, flowerbed:list[int] , n:int) -> None:
        self.flowerbed = flowerbed
        self.n = n

class Case:
    def __init__(self, name: str, args: Arg, want: bool):
        self.name = name
        self.args = args
        self.want = want
    

class TestSolution(unittest.TestCase):
    def test_solution(self):
        solution = s.Solution()
        cases = [
            Case("case: 1", Arg([1,0,0,0,1], 1), True),
            Case("case: 2", Arg([1,0,0,0,1], 2), False),
            Case("case: 3", Arg([0], 1), True),
            Case("case: 4", Arg([1], 1), False),
            Case("case: 5", Arg([], 0), True),
            Case("case: 6", Arg([], 1), False),
            Case("case: 7", Arg([1,0,0,0,0,1], 2), False),
            Case("case: 8", Arg([0,0,1,0,0], 1), True),
            Case("case: 9", Arg([1,0,1,0,1,0,1], 0), False),
        ]

        for case in cases:
            arg = case.args
            ans = solution.canPlaceFlowers(arg.flowerbed,arg.n)
            try:
                self.assertEqual(ans, case.want)
            except:
                print("error")
                raise

if __name__ == '__main__':
    unittest.main()