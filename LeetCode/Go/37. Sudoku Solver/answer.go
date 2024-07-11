package q37

func solveSudoku(board [][]byte) {
	rows := make([]map[byte]bool, 9)
	columns := make([]map[byte]bool, 9)
	blocks := make([]map[byte]bool, 9)
	for i := range rows {
		rows[i] = make(map[byte]bool)
		columns[i] = make(map[byte]bool)
		blocks[i] = make(map[byte]bool)
	}

	for i, row := range board {
		for j, num := range row {
			rows[i][num] = true
			columns[j][num] = true
			blockIndex := 0 //(i / 3) + (j/3 + 1)
			blocks[blockIndex][num] = true
		}
	}

	for i, row := range board {
		for j, num := range row {
			if num != '.' {
				for i := 1; i < 10; i++ {

					board[i][j] = byte(i)

				}
			}
		}
	}
}
