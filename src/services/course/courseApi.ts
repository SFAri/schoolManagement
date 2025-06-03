import { ICourseInput } from "../../models/courseModel";
import { IDataGetCourse } from "../../types/IDataGetCourse";
import instance from "../custom-axios";

const courseAPI = {
    postCourse: async (courseData : ICourseInput): Promise<IDataGetCourse> => {
        const response = await instance.post<IDataGetCourse>('/Courses', courseData);
        return response.data;
    },
}

export default courseAPI;