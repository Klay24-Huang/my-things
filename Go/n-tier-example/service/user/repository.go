package user

type UserRepository struct {
	// should have a db here
}

func NewRepository() *UserRepository {
	return &UserRepository{}
}

func (u *UserRepository) GetById(id int) User {
	return User{
		Id:   1,
		Name: "Nick",
	}
}
