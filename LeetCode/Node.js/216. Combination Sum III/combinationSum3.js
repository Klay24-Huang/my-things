var combinationSum3 = function (k, n) {
    let ans = []
    let currTemp = []
    let i = 1
    // hook
    let j = 1
    let sum = 0

    let goPrev = () => {
        // console.log("before goPrev", i, sum, currTemp, ans)
        let popNum = currTemp.pop()
        sum -= popNum
        // let newArr = currTemp.slice()
        i = popNum

        // console.log("after goPrev", i, j, sum, currTemp, ans)
        // console.log("after goPrev", i, sum, currTemp, ans)
    }

    let goByHook = () => {
        // console.log("before hook", i, j, sum, currTemp, ans)
        let popNum = currTemp.pop()
        while (popNum != j) {
            sum -= popNum
            // i = popNum
            popNum = currTemp.pop()
        }

        // find hook
        sum -= popNum
        i = popNum + 1
        j = i
        // console.log("after hook", i, sum, currTemp, ans)
    }

    while (i <= 9) {
        if (currTemp.length === 0) {
            j = i
        }

        console.log("start", i, j, sum, currTemp, ans)
        if (i > n) {
            break;
        }

        if (sum + i > n) {
            goByHook()
            // console.log("after go by hook：", i, ",", sum, currTemp, ans)
        }

        if (i === 9 && sum + i < n) {
            goByHook()
        }

        if (currTemp.length < k) {
            currTemp.push(i)
            sum += i
        }

        if (currTemp.length == k) {
            if (i == 9 && sum < n) {
                goPrev()
            }

            // console.log('sum', sum)
            if (sum == n) {
                ans.push(currTemp.slice())
                // console.log("add ans", i, ",", sum, currTemp, ans)
                goPrev()
            }

            if (sum > n) {
                // console.log("break", sum)
                break
            }

            goPrev()
        }

        // if (i === 9 && sum < n) {
        //     goPrev()
        //     goPrev()
        // }

        i++
    }
    // console.log('return ans', ans)
    return ans
};

export default combinationSum3