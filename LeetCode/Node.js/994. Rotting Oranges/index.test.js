import orangesRotting from '.'

it('test orange', () => {
    const tests = [{
            title: 'case 1',
            input: [
                [2, 1, 1],
                [1, 1, 0],
                [0, 1, 1]
            ],
            expected: 4
        },
        {
            title: 'case 2',
            input: [
                [2, 1, 1],
                [0, 1, 1],
                [1, 0, 1]
            ],
            expected: -1
        },
        {
            title: 'case 3',
            input: [
                [0, 2]
            ],
            expected: 0
        },
        {
            title: 'case 4',
            input: [
                [1, 2]
            ],
            expected: 1
        },
    ]

    for (const test of tests) {
        console.info(test.title)
        try {
            const ans = orangesRotting(test.input)
            expect(ans).toBe(test.expected)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest '994. Rotting Oranges/index.test.js'