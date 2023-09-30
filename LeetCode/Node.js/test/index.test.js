import foo from "."

it('test nearest exit', () => {
    const tests = [{}]

    for (const test of tests) {
        var ans
        try {
            ans = foo()
            expect(ans).toBe(1)
        } catch (error) {
            throw error
        }
    }
})


// yarn jest 'test/index.test.js'