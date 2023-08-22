class Solution:
    def mergeAlternately(self, word1: str, word2: str) -> str:
        length_one = len(word1)
        length_two = len(word2)

        arr = []

        for i in range(max(length_one, length_two)):
            if i < length_one:
                arr.append(word1[i])
            if i < length_two:
                arr.append(word2[i])

        return ''.join(arr)