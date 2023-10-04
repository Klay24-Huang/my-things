import findMinArrowShots from '.'

it('test find min arrows', () => {
    const tests = [{
            title: 'case 1',
            input: [
                [10, 16],
                [2, 8],
                [1, 6],
                [7, 12]
            ],
            expected: 2
        },
        {
            title: 'case 2',
            input: [
                [1, 2],
                [3, 4],
                [5, 6],
                [7, 8]
            ],
            expected: 4
        },
        {
            title: 'case 3',
            input: [
                [1, 2],
                [2, 3],
                [3, 4],
                [4, 5]
            ],
            expected: 2
        },
    ]

    for (const test of tests) {
        console.info(test.title)
        try {
            const ans = findMinArrowShots(test.input)
            expect(ans).toBe(test.expected)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest '452. Minimum Number of Arrows to Burst Balloons/index.test.js'