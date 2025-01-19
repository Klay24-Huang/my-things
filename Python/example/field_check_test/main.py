from typing import Annotated, Any, Callable

from pydantic import BaseModel, ValidationError, Field
from pydantic_core import PydanticCustomError

# 自定義錯誤消息的函數
def custom_error_msg(exc_factory: Callable[[str | None, Exception], Exception]) -> Any:
    def _validator(v: Any, next_: Any) -> Any:
        try:
            return next_(v)
        except Exception as e:
            raise exc_factory('name', e) from None

    return _validator


# 使用 Field 並添加正則表達式約束
NameString = Annotated[
    str,
    Field(
        pattern=r"^[0-9~`!@#$%^&*()_+={\[}\]|\:;\"<>\/?]*$",
        description="This field must only contain certain special symbols."
    ),
    custom_error_msg(
        lambda field_name, _: PydanticCustomError(
            "str_error",
            f"The field {field_name} cannot contain special symbols.",
        )
    ),
]

class Model(BaseModel):
    name: NameString

try:
    dog = Model(name="dog123")  # 這裡會引發驗證錯誤
except ValidationError as e:
    print(e.errors())
