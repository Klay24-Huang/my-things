import moveZeroes from './moveZeroes'
import deepClone from '../utils/deepClone'



it('test move zeroes function', () => {
    const tests = [{
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

    for (const test of tests) {
        var ans
        try {
            const input = deepClone(test.input)
            ans = moveZeroes(input)
            expect(ans).toStrictEqual(test.expect)
        } catch (error) {
            const message = `
                    ${test.title},
                    input: ${test.input},
                    ans: ${ans},
                    expect: ${test.expect}`
            throw new Error(message)
        }
    }
})

// test('move zeroes', () => {
//     for (const test of tests) {
//         var input = deepClone(test.input)
//         var ans = moveZeroes(input)
//         var message = `${test.title},
//         input: ${test.input},
//         ans: ${ans},
//         expect: ${test.expect}`
//         console.log(message)
//         expect(ans).toStrictEqual(test.expect)
//     }
// })


// yarn jest '283. Move Zeroes/moveZeroes.test.js'