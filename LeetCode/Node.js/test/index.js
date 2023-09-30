var foo = () => {
    throw new Error("my error")
    return 1 / 0
}

export default foo