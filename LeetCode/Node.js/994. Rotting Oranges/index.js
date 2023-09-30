var orangesRotting = function (grid) {
    let q = []
    let freshOrangeCount = 0;
    let minutes = 0

    for (let i = 0; i < grid.length; i++) {
        for (let j = 0; j < grid[i].length; j++) {
            const orange = grid[i][j];
            // rotten
            if (orange === 2) {
                q.push([i, j])
                continue
            }

            if (orange === 1) {
                freshOrangeCount++
                continue
            }
        }
    }
    // console.log("before rotten", q, freshOrangeCount)
    const dirs = [
        [-1, 0],
        [1, 0],
        [0, 1],
        [0, -1]
    ]
    const rotten = () => {
        // console.log("before rotten", q)
        let newQ = []
        for (const rottenOrange of q) {
            for (const dir of dirs) {
                const newPosition = [rottenOrange[0] + dir[0], rottenOrange[1] + dir[1]]
                const x = newPosition[0]
                const y = newPosition[1]
                // console.log('new position', newPosition)
                // out of range
                if (x < 0 || x === grid.length || y < 0 || y === grid[rottenOrange[0]].length) {
                    continue
                }
                // console.log("orange", grid[x][y], newPosition)
                if (grid[x][y] === 1) {
                    // console.log('rotten orange', newPosition)
                    newQ.push(newPosition)
                    grid[x][y] = 2
                    freshOrangeCount--
                }
            }
        }

        // console.log('new q', newQ)
        q = newQ
        if (q.length > 0) {
            minutes++
            rotten()
        }
    }

    rotten()
    // console.log('before return', grid, freshOrangeCount, minutes)
    return freshOrangeCount === 0 ? minutes : -1
};

export default orangesRotting