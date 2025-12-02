export interface IUpdateNews {
    id: string;
    title: string;
    topic: string;
    author: string;
    content: string;
    createdAt: string | null;
    imagePath: string;
    image: File;
}
