import minFlips from '.'

it('test erase', () => {
    const tests = [{
            title: 'case 1',
            input: {
                a: 2,
                b: 6,
                c: 5,
            },
            expected: 3
        },
        {
            title: 'case 2',
            input: {
                a: 4,
                b: 2,
                c: 7,
            },
            expected: 1
        },
        {
            title: 'case 3',
            input: {
                a: 1,
                b: 2,
                c: 3,
            },
            expected: 0
        },
        {
            title: 'case 3',
            input: {
                a: 8,
                b: 3,
                c: 5,
            },
            expected: 3
        },
    ]

    for (const test of tests) {
        console.info(test.title)
        try {
            const ans = minFlips(test.input.a, test.input.b, test.input.c)
            expect(ans).toBe(test.expected)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest '1318. Minimum Flips to Make a OR b Equal to c/index.test.js'