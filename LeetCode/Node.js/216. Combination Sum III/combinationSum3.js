var combinationSum3 = function (k, n) {
    let ans = []
    let currArr = []
    let i = 1
    let sum = 0

    let goPrev = () => {
        let popNum = currArr.pop()
        sum -= popNum
        i = popNum

        // console.log("after goPrev", i, sum, currArr, ans)
    }

    while (i <= 9) {
        // console.log("start", i, sum, currArr, ans)
        if (currArr.length < k) {
            currArr.push(i)
            sum += i
        }

        if (currArr.length < k && sum > n) {
            break
        }

        if (currArr.length < k && sum + i + 1 > n) {
            goPrev()
            goPrev()
        }

        if (currArr.length === k && sum === n) {
            ans.push(currArr.slice())
            // console.log("add ans", i, sum, currArr, ans)
            goPrev()
            goPrev()
        }

        if (currArr.length <= k && sum < n && i === 9) {
            goPrev()
            goPrev()
        }

        // if (currArr.length <= k && sum < n && i === 9) {
        //     console.log("should in", i, sum, currArr, ans)
        //     goPrev()
        // }

        if (currArr.length === k && sum < n) {
            goPrev()
        }

        i++
    }

    return ans
};

export default combinationSum3