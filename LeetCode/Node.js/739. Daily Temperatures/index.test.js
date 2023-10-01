import dailyTemperatures from '.'

it('test orange', () => {
    const tests = [{
            title: 'case 1',
            input: [73, 74, 75, 71, 69, 72, 76, 73],
            expected: [1, 1, 4, 2, 1, 1, 0, 0]
        },
        {
            title: 'case 2',
            input: [30, 40, 50, 60],
            expected: [1, 1, 1, 0]
        },
        {
            title: 'case 3',
            input: [30, 60, 90],
            expected: [1, 1, 0]
        },
        {
            title: 'case 4',
            input: [89, 62, 70, 58, 47, 47, 46, 76, 100, 70],
            expected: [8, 1, 5, 4, 3, 2, 1, 1, 0, 0]
        },

    ]

    for (const test of tests) {
        console.info(test.title)
        try {
            const ans = dailyTemperatures(test.input)
            expect(ans).toEqual(test.expected)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest '739. Daily Temperatures/index.test.js'