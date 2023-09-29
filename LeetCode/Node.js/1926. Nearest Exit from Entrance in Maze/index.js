var nearestExit = function (maze, entrance) {
    // step
    entrance[2] = 0
    maze[entrance[0]][entrance[1]] = 0

    let outOfMaze = (entrance) => {
        // // // // // // console.log('in out of Maze function', (entrance[0] in maze && entrance[1] in maze[entrance[0]]))
        if (entrance[0] in maze && entrance[1] in maze[entrance[0]]) return false
        return true
    }

    let isWall = (entrance) => {
        if (maze[entrance[0]]
            [entrance[1]] ===
            "+") return true

        return false
    }

    let q = []
    q.push(entrance)
    let dirs = ["U", "R", "D", "L"]

    while (q.length > 0) {
        // console.log(q)
        let newQ = []
        for (let position of q) {
            for (let dir of dirs) {
                let newPosition = position.slice()
                newPosition[2]++
                switch (dir) {
                    case "U":
                        newPosition[0]--
                        break;
                    case "R":
                        newPosition[1]++
                        break;
                    case "D":
                        newPosition[0]++
                        break;
                    case "L":
                        newPosition[1]--
                        break;
                    default:
                        break;
                }

                let isOutOfMaze = outOfMaze(newPosition)
                let prevStep = newPosition[2] - 1
                // is ans
                if (isOutOfMaze && prevStep > 0) {
                    return prevStep
                }

                if (isOutOfMaze) {
                    continue
                }

                let isVisited = maze[newPosition[0]][newPosition[1]] < newPosition[2]

                if (!isWall(newPosition) && !isVisited) {
                    newQ.push(newPosition)
                    // record way
                    maze[newPosition[0]][newPosition[1]] = newPosition[2]
                }
                // console.log('maze', maze)
            }
        }
        q = newQ
    }
    return -1
}

export default nearestExit