import { useState } from "react";

type Count = {
  value: number;
};

function StateExample() {
  const [count, setCount] = useState<Count>({ value: 0 });
  const onClick = () => {
    setCount(count => ({
      ...count,
      ...{
        value: count.value + 1
      }
    }))
  };

  return (
    <div>
      <div>{count && count.value}</div>
      <button onClick={onClick}>Hi</button>
    </div>
  );
}

export default StateExample;
