import logo from "./logo.svg";
import "./App.css";
import { BrowserRouter, Routes, Route } from "react-router-dom";
import Upload from "./Components/Upload";
import Show from "./Components/Show";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Upload />} />
        <Route path="/show" element={<Show />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
