from typing import List

class Solution:
    def countSubIslands(self, grid1: List[List[int]], grid2: List[List[int]]) -> int:
        lenOfi = len(grid1)
        lenOfj = len(grid1[0])
        # print(grid2[0][-1])
        countOfValidIslands = 0
        stack = []
        def CheckIsland ():
            nonlocal countOfValidIslands
            isValidIsland = True
            while len(stack) > 0:
                i, j = stack.pop()
                # print(i, j)
                grid2[i][j] = 0
                if (grid1[i][j] != 1):
                    isValidIsland = False
                # go up 
                GoNextStep(i-1, j)

                # go right
                GoNextStep(i, j+1)

                #go down
                GoNextStep(i+1, j)
                
                #go left
                GoNextStep(i, j-1)
                
            if isValidIsland:
                countOfValidIslands += 1
        
        def GoNextStep(i , j):
            # prevent out of index
            try :
                # in python list[-1] will return last element, not trigger out of index
                if i < 0 or j < 0 or i == lenOfi or j == lenOfj:
                    return
                
                if (grid2[i][j] == 1):
                    # print(f'push {i, j}')
                    stack.append([i,j])
            except:
                return


        for i in range(len(grid1)):
            for j in range(len(grid1[0])):
                if grid2[i][j] == 1:
                    stack.append([i, j])
                    CheckIsland()
        
        return countOfValidIslands
