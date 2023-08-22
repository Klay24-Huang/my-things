import moveZeroes from './moveZeroes'

var tests = [{
        title: 'case 1',
        input: [0, 1, 0, 3, 12],
        expect: [1, 3, 12, 0, 0]
    },
    {
        title: 'case 2',
        input: [0],
        expect: [0]
    },
]

// for (const test of tests) {
//     test(test.title, () => {
//         expect(moveZeroes(test.input)).toBe(test.expect)
//     })
// }

test('move zeroes', () => {
    for (const test of tests) {
        var ans = moveZeroes(test.input)
        var message = `${test.title},
        input: ${test.input},
        ans: ${ans},
        expect: ${test.expect}`
        console.log(message)
        expect(ans).toBe(test.expect)
    }
})