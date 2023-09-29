import canVisitAllRooms from '.'

it('test can visit all rooms', () => {
    const tests = [{
        title: 'case 1',
        input: [
            [1],
            [2],
            [3],
            []
        ],
        expected: true
    }, {
        title: 'case 2',
        input: [
            [1, 3],
            [3, 0, 1],
            [2],
            [0]
        ],
        expected: false
    }]

    for (const test of tests) {
        var ans
        try {
            ans = canVisitAllRooms(test.input)
            expect(ans).toBe(test.expected)
        } catch (error) {
            const message = `
                    ${test.title},
                    
                    ans: ${ans},
                    expect: ${test.expected}`
            console.error(message)
            throw new Error(error)
        }
    }
})


// yarn jest '841. Keys and Rooms/index.test.js'