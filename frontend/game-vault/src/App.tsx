import { BrowserRouter, Routes, Route } from "react-router-dom"
import HomePage from "./pages/HomePage"
import NavBar from "./components/NavBar"
import PlatformPage from "./pages/PlatformPage"
import GamePage from "./pages/GamePage"
import SearchResultsPage from "./pages/SearchResultsPage"

function App() {
    return (
        <BrowserRouter>
            <div className="min-h-screen bg-gray-950 text-white">
                <NavBar />
                <Routes>
                    <Route path="/platform/:name" element={<PlatformPage />} />
                    <Route path="/" element={<HomePage />} />
                    <Route path="/games/:id" element={<GamePage />} />
                    <Route path="/search" element={<SearchResultsPage />} />
                </Routes>
            </div>
        </BrowserRouter>
    )
}

export default App
