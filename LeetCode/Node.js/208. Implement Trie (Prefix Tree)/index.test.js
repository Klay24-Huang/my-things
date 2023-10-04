import Trie from '.'

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