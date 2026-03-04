import { useState, useEffect } from "react"
import { useParams } from "react-router-dom"
import { getGames } from "../services/api"
import GameCard from "../components/GameCard"
import Pagination from "../components/Pagination"
import LoadingThrobber from "../components/LoadingThrobber"
import ErrorMessage from "../components/ErrorMessage"
import type { Game } from "../types/game"
import { formatPlatformName } from "../utils/formatPlatformName"

const PAGE_SIZE = 20

function PlatformPage() {
    const { name } = useParams()
    const [games, setGames] = useState<Game[]>([])
    const [page, setPage] = useState(1)
    const [totalCount, setTotalCount] = useState(0)
    const [error, setError] = useState("")
    const [loading, setLoading] = useState(true)

    useEffect(() => {
        if (!name) return
        setLoading(true)
        setError("")
        //populate with paginated response values
        getGames(page, PAGE_SIZE, name)
            .then((data) => {
                setGames(data.items)
                setTotalCount(data.totalCount)
            })
            .catch(() => setError("Failed to load games"))
            .finally(() => setLoading(false))
    },[name, page])

    const totalPages = Math.ceil(totalCount / PAGE_SIZE)

    if (loading) return <LoadingThrobber />
    if (error) return <ErrorMessage message={error} onRetry={() => window.location.reload()} />

    return (
        <main className="max-w-4xl mx-auto px-6 py-8">
            <h1 className="text-3xl font-bold mb-2">{formatPlatformName(name ?? "")}</h1>
            <p className="text-gray-400 mb-6">{totalCount.toLocaleString()} games</p>
            <div className="flex flex-col gap-3">
                {games.map((game) => (
                    <GameCard key={game.id} game={game} />
                ))}
            </div>
            <Pagination page={page} totalPages={totalPages} onPageChange={setPage} />
        </main>
    )
}

export default PlatformPage
