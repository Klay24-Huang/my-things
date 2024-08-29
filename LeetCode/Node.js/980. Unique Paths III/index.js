/**
 * @param {number[][]} grid
 * @return {number}
 */
var uniquePathsIII = function (grid) {
	let [numberOfEmptySquares, positionOfRobot] =
		countEmptySquaresAndFindRobot(grid)

	return goThroughGrid(grid, positionOfRobot, numberOfEmptySquares + 1) // go to ending square need one step
}

var countEmptySquaresAndFindRobot = (grid) => {
	let countOfEmptySquares = 0
	let positionOfRobot
	grid.forEach((subArray, i) => {
		subArray.forEach((square, j) => {
			if (square === 0) {
				countOfEmptySquares++
			}

			if (square === 1) {
				positionOfRobot = [i, j]
			}
		})
	})
	return [countOfEmptySquares, positionOfRobot]
}

var goThroughGrid = (grid, positionOfRobot, stepsNeedToGo) => {
	let [i, j] = positionOfRobot
	grid[i][j] = -1
	stepsNeedToGo--
	return (
		nextStep(grid, [i - 1, j], stepsNeedToGo) + // up
		nextStep(grid, [i + 1, j], stepsNeedToGo) + // down
		nextStep(grid, [i, j + 1], stepsNeedToGo) + // right
		nextStep(grid, [i, j - 1], stepsNeedToGo) // left
	)
}

var nextStep = (grid, positionOfRobot, stepsNeedToGo) => {
	let [i, j] = positionOfRobot
	let [m, n] = [grid.length - 1, grid[0].length - 1]
	if (i > m || j > n || i < 0 || j < 0) {
		return 0 // out of grid
	}
	// console.log([i, j])
	if (grid[i][j] === -1) {
		return 0 // obstacles
	}

	if (grid[i][j] === 2 && stepsNeedToGo === 0) {
		return 1
	}
	return goThroughGrid(deepCopy2DArray(grid), positionOfRobot, stepsNeedToGo)
}

var deepCopy2DArray = (grid) => {
	return grid.map((subArray) => subArray.slice())
}
