import eraseOverlapIntervals from '.'

it('test erase', () => {
    const tests = [{
            title: 'case 1',
            input: [
                [1, 2],
                [2, 3],
                [3, 4],
                [1, 3]
            ],
            expected: 1
        },
        {
            title: 'case 2',
            input: [
                [1, 2],
                [1, 2],
                [1, 2]
            ],
            expected: 2
        },
        {
            title: 'case 3',
            input: [
                [1, 2],
                [2, 3]
            ],
            expected: 0
        },
        {
            title: 'case 4',
            input: [
                [1, 100],
                [11, 22],
                [1, 11],
                [2, 12]
            ],
            expected: 2
        },
        {
            title: 'case 5',
            input: [
                [0, 2],
                [1, 3],
                [2, 4],
                [3, 5],
                [4, 6]
            ],
            expected: 2
        },
        {
            title: 'case 6',
            input: [
                [-52, 31],
                [-73, -26],
                [82, 97],
                [-65, -11],
                [-62, -49],
                [95, 99],
                [58, 95],
                [-31, 49],
                [66, 98],
                [-63, 2],
                [30, 47],
                [-40, -26]
            ],
            expected: 7
        },
        {
            title: 'case 7',
            input: [
                [-52, 31],
                [-73, -26],
                [82, 97],
                [-65, -11],
                [-62, -49],
                [95, 99],
                [46, 95],
                [-31, 49],
                [66, 98],
                [-63, 2],
                [30, 47],
                [-40, -26]
            ],
            expected: 8
        },
    ]

    for (const test of tests) {
        console.info(test.title)
        try {
            const ans = eraseOverlapIntervals(test.input)
            expect(ans).toBe(test.expected)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest '435. Non-overlapping Intervals/index.test.js'