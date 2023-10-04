/**
 * @param {number[][]} points
 * @return {number}
 */
var findMinArrowShots = function (points) {
    points.sort((a, b) => a[1] - b[1])
    let arrow = 0
    let pointHook = -1
    for (const point of points) {
        if (pointHook === -1) {
            pointHook = point[1]
            arrow++
            continue
        }

        // not overlap 
        if (point[0] > pointHook) {
            pointHook = point[1]
            arrow++
            continue
        }
    }
    return arrow
};

export default findMinArrowShots