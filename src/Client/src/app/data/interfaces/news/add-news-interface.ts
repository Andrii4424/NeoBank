export interface IAddNews {
    id: string | null;
    title: string;
    topic: string;
    author: string;
    content: string;
    createdAt: string | null;
    image: File | null;
}