import { Link } from "react-router-dom"

interface PlatformCardProps {
    name: string
    gameCount: number
}

function PlatformCard( { name, gameCount }: PlatformCardProps) {
    return (
        <Link to={`/platform/${name}`}
            className="bg-gray-800 rounded-lg p-6 text-center 
                        hover:ring-2 hover:ring-amber-500 hover:-translate-y-1
                        transition-all duration-200 cursor-pointer"
        >
            <h2 className="text-lg font-semibold text-white">{name}</h2>
            <p className="text-sm text-gray-400 mt-2">{gameCount.toLocaleString()} games</p>
        </Link>
    )
}

export default PlatformCard
