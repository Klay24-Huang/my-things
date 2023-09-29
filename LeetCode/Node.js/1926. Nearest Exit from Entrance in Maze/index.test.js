import nearestExit from "."

it('test nearest exit', () => {
    const tests = [{
            title: 'case 1',
            input: {
                maze: [
                    ["+", "+", ".", "+"],
                    [".", ".", ".", "+"],
                    ["+", "+", "+", "."]
                ],
                entrance: [1, 2]
            },
            expected: 1
        },
        {
            title: "case 2",
            input: {
                maze: [
                    ["+", "+", "+"],
                    [".", ".", "."],
                    ["+", "+", "+"]
                ],
                entrance: [1, 0]
            },
            expected: 2
        },
        {
            title: "case 3",
            input: {
                maze: [
                    [".", "+"],
                ],
                entrance: [0, 0]
            },
            expected: -1
        },
        {
            title: "case 4",
            input: {
                maze: [
                    [".", "."],
                ],
                entrance: [0, 1]
            },
            expected: 1
        },
        {
            title: "case 5",
            input: {
                maze: [
                    ["."],
                    ["."],
                    ["."],
                    ["."]
                ],
                entrance: [2, 0]
            },
            expected: 1
        },
        {
            title: "case 6",
            input: {
                maze: [
                    ["+", ".", "+", "+", "+", "+", "+"],
                    ["+", ".", "+", ".", ".", ".", "+"],
                    ["+", ".", "+", ".", "+", ".", "+"],
                    ["+", ".", ".", ".", ".", ".", "+"],
                    ["+", "+", "+", "+", ".", "+", "."]
                ],
                entrance: [0, 1]
            },
            expected: 7
        },
        {
            title: "case 7",
            input: {
                maze: [
                    ["."]
                ],
                entrance: [0, 0]
            },
            expected: -1
        },
    ]

    for (const test of tests) {
        var ans
        try {
            ans = nearestExit(test.input.maze, test.input.entrance)
            expect(ans).toBe(test.expected)
        } catch (error) {
            const message = `
                    ${test.title},`
            console.log(message)
            throw new Error(error)
        }
    }
})


// yarn jest '1926. Nearest Exit from Entrance in Maze/index.test.js'