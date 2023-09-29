var canVisitAllRooms = function (rooms) {
    let roomSet = new Set()
    roomSet.add(0)

    let queue = [0]
    goRoom(rooms, roomSet, queue)

    // console.log("set before return", roomSet)
    for (let index = 0; index < rooms.length; index++) {
        if (!roomSet.has(index)) return false
    }
    return true
}

var goRoom = (rooms, roomSet, queue) => {
    // console.log('this queue', queue)
    let newQueue = []
    for (let roomId of queue) {
        // console.log('check', rooms[roomId], roomId)
        rooms[roomId].forEach(nextRoomId => {
            // console.log('next room id', nextRoomId)
            if (!roomSet.has(nextRoomId)) {
                newQueue.push(nextRoomId)
                roomSet.add(nextRoomId)
            }
        })
    }

    // console.log('go next room function', newQueue)
    if (newQueue.length === 0) return

    goRoom(rooms, roomSet, newQueue)
}


export default canVisitAllRooms