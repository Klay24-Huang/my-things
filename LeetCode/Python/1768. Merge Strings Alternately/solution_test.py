import unittest
import solution as s

class Case:
    def __init__(self, word1, word2, answer):
        self.word1 = word1
        self.word2 = word2
        self.answer = answer
    

class TestSolution(unittest.TestCase):
    def test_solution(self):
        solution = s.Solution()
        cases = [
            Case("abc", "pqr", "apbqcr")
        ]

        for case in cases:
            ans = solution.mergeAlternately(case.word1, case.word2)
            self.assertEqual(ans, case.answer)

if __name__ == '__main__':
    unittest.main()