from typing import Union

from fastapi import FastAPI, Request, HTTPException

app = FastAPI()


@app.get("/")
async def read_root():
    return {"Hello": "World"}


@app.get("/items/{item_id}")
async def read_item(item_id: int, q: Union[str, None] = None):
    return {"item_id": item_id, "q": q}

@app.post("/upload")
async def upload_large_request(request: Request):
    # 接收完整的原始數據
    body = await request.body()

    # # 確保數據大小不超過預期範圍（可選的安全檢查）
    # if len(body) > 10 * 1024 * 1024:  # 10 MB
    #     raise HTTPException(status_code=413, detail="Payload too large")

    return {"message": "Request received", "size": len(body)}