import axios, { AxiosResponse } from "axios";
import { IActivity } from "../models/activity";

axios.defaults.baseURL = "http://localhost:50000/api/v1";

const responseBody = (response: AxiosResponse) => response.data;

const sleep = (ms: number) => (response: AxiosResponse) => 
  new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms));

const requests = {
  get: (url: string) => axios.get(url).then(sleep(1000)).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(sleep(1000)).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(sleep(1000)).then(responseBody),
  del: (url: string) => axios.delete(url).then(sleep(1000)).then(responseBody),
}

const Activities = {
  list: (): Promise<IActivity[]> => requests.get(`/activities/list`),
  details: (id: string) => requests.get(`/activities/details/${id}`),
  create: (activity: IActivity) => requests.post(`/activities/create`, activity),
  edit: (activity: IActivity) => requests.put(`/activities/edit/${activity.id}`, activity),
  delete: (id: string) => requests.del(`/activities/delete/${id}`),
}

const agent = {
  Activities
};

export default agent;