import combinationSum3 from './combinationSum3'
import deepClone from '../utils/deepClone'



it('test combinationSum3 function', () => {
    const tests = [

        // {
        //     title: 'case 1',
        //     input: {
        //         k: 4,
        //         n: 1,
        //     },
        //     expect: []
        // },
        // {
        //     title: 'case 2',
        //     input: {
        //         k: 3,
        //         n: 7,
        //     },
        //     expect: [
        //         [1, 2, 4]
        //     ]
        // },
        // {
        //     title: 'case 3',
        //     input: {
        //         k: 2,
        //         n: 6,
        //     },
        //     expect: [
        //         [1, 5],
        //         [2, 4]
        //     ]
        // },
        // {
        //     title: 'case 4',
        //     input: {
        //         k: 3,
        //         n: 7,
        //     },
        //     expect: [
        //         [1, 2, 4],
        //     ]
        // },
        // {
        //     title: 'case 5',
        //     input: {
        //         k: 3,
        //         n: 15,
        //     },
        //     expect: [
        //         [
        //             [1, 5, 9],
        //             [1, 6, 8],
        //             [2, 4, 9],
        //             [2, 5, 8],
        //             [2, 6, 7],
        //             [3, 4, 8],
        //             [3, 5, 7],
        //             [4, 5, 6]
        //         ]
        //     ]
        // },
        {
            title: 'case 6',
            input: {
                k: 4,
                n: 24,
            },
            expect: [
                [1, 6, 8, 9],
                [2, 5, 8, 9],
                [2, 6, 7, 9],
                [3, 4, 8, 9],
                [3, 5, 7, 9],
                [3, 6, 7, 8],
                [4, 5, 6, 9],
                [4, 5, 7, 8]
            ]
        },

    ]

    for (const test of tests) {
        var ans
        try {
            ans = combinationSum3(test.input.k, test.input.n)
            expect(ans).toStrictEqual(test.expect)
        } catch (error) {
            const message = `
                    ${test.title},
                    
                    ans: ${ans},
                    expect: ${test.expect}`
            console.log(message)
            throw new Error(error)
        }
    }
})


// yarn jest '216. Combination Sum III/combinationSum3..test.js'