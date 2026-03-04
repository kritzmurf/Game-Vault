import { useState, useEffect } from "react"
import { useSearchParams } from "react-router-dom"
import { searchGames } from "../services/api"
import GameCard from "../components/GameCard"
import Pagination from "../components/Pagination" 
import ErrorMessage from "../components/ErrorMessage"
import LoadingThrobber from "../components/LoadingThrobber"
import type { Game } from "../types/game"

const PAGE_SIZE = 20

function SearchResultsPage() {
    const [searchParams] = useSearchParams()
    const q = searchParams.get("q") ?? ""
    const [games, setGames] = useState<Game[]>([])
    const [page, setPage] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [error, setError] = useState("")
    const [loading, setLoading] = useState(false)

    useEffect(() => {
        if (!q) return
        setLoading(true)
        setError("")
        searchGames(q, page, PAGE_SIZE)
            .then((data) => {
                setGames(data.items)
                setTotalCount(data.totalCount)
            })
            .catch(() => setError("Failed to search games"))
            .finally(() => setLoading(false))
    }, [q, page])

    const totalPages = Math.ceil(totalCount / PAGE_SIZE)

    if (loading) return <LoadingThrobber />
    if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />

    return (
        <main className = "max-w-4xl mx-auto px-6 py-8">
            <h1 className="text-3xl font-bold mb-2">Results for "{q}"</h1>
            <p className="text-gray-400 mb-6">{totalCount.toLocaleString()} results</p>
            <div className="flex flex-col gap-3">
                {games.map((game) => (
                    <GameCard key={game.id} game={game} />
                ))}
            </div>
            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </main>
    )
}

export default SearchResultsPage
