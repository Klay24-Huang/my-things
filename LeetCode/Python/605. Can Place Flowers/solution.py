class Solution:
    def canPlaceFlowers(self, flowerbed: list[int], n: int) -> bool:
        if n == 0:
            return True
        bedLen = len(flowerbed)
        for i in range(bedLen):
            if flowerbed[i] == 0:
                if i-1 >= 0 and flowerbed[i-1] == 1:
                    continue

                if i+1 < bedLen and flowerbed[i+1] == 1:
                    continue

                flowerbed[i] = 1
                n = n-1

                if n == 0:
                    break

        return n == 0