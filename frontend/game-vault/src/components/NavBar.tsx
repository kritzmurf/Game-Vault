import { useState } from "react"
import { Link, useNavigate } from "react-router-dom"

function NavBar() {
    const [query, setQuery] = useState("")
    const navigate = useNavigate()

    function handleSearch(e: React.KeyboardEvent) {
        if (e.key === "Enter" && query.trim()) {
            navigate(`/search?q=${encodeURIComponent(query.trim())}`)
        }
    }
    return (
        <nav className="bg-gray-900 border-b border-gray-800 px-6 py-4 flex items-center justify-between">
            <Link to="/" className="text-2xl font-bold text-amber-500 hover:text-amber-400">
                Game Vault
            </Link>
            <input
                type="text"
                placeholder="Search games.."
                value={query}
                onChange={(e) => setQuery(e.target.value)}
                onKeyDown={handleSearch}
                className="bg-gray-800 text-white placeholder-gray-500 px-4 py-2 rounded-lg w-64 focus:outline-none focus:ring-2 focus:ring-amber-500"
            />
        </nav>
    )
}

export default NavBar
