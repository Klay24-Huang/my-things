import removeStars from './removeStars'

it('remove stars function', () => {
    const tests = [{
            title: 'case 1',
            input: 'leet**cod*e',
            expected: 'lecoe',
        },
        {
            title: 'case 2',
            input: 'erase*****',
            expected: ''
        }
    ]

    for (const test of tests) {
        var ans
        try {
            ans = removeStars(test.input)
            expect(ans).toEqual(test.expected)
        } catch (error) {
            const message = `
                    ${test.title},
                    ans: ${ans},
                    expect: ${test.expected}`
            throw new Error(message)
        }
    }
})