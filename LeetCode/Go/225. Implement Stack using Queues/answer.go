package q225

type MyStack struct {
	array []int
}

func Constructor() MyStack {
	return MyStack{array: []int{}}
}

func (this *MyStack) Push(x int) {
	this.array = append([]int{x}, this.array...)
}

func (this *MyStack) Pop() int {
	tmp := this.array[0]
	this.array = this.array[1:]
	return tmp
}

func (this *MyStack) Top() int {
	return this.array[0]
}

func (this *MyStack) Empty() bool {
	return len(this.array) == 0
}
