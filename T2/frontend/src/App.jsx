import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Create from "./Components/Create";
import Home from "./Components/Home";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/create" element={<Create />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
