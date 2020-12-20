import { IPhoto, IProfile } from './../models/profile';
import { IUser, IUserFormValues } from './../models/user';
import axios, { AxiosResponse } from "axios";
import { toast } from "react-toastify";
import { history } from "../..";
import { IActivitiesEnvelope, IActivity } from "../models/activity";

axios.defaults.baseURL = process.env.REACT_APP_API_URL;

axios.interceptors.request.use((config) => {
  const token = window.localStorage.getItem('jwt');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
}, error => {
  return Promise.reject(error);
});

axios.interceptors.response.use(undefined, error => {
  if (error.message === 'Network Error' && !error.response) {
    toast.error('Network error - make sure API is running!');
  }
  const { status, data, config, headers } = error.response;
  if (status === 404) {
    history.push('/notfound');
  }
  if (status === 401 && headers['www-authenticate'] !== '' && headers['content-length'] === '0') {
    window.localStorage.removeItem('jwt');
    history.push('/');
    toast.info('Your session has expired, please login again');
  }
  if (status === 400 && config.method === 'get' && data.errors.hasOwnProperty('id')) {
    history.push('/notfound');
  }
  if (status === 500) {
    toast.error('Server error - check the terminal for more info!');
  }
  throw error.response;
});

const responseBody = (response: AxiosResponse) => response.data;

// const sleep = (ms: number) => (response: AxiosResponse) =>
//   new Promise<AxiosResponse>(resolve => setTimeout(() => resolve(response), ms));

const requests = {
  get: (url: string) => axios.get(url).then(responseBody),
  post: (url: string, body: {}) => axios.post(url, body).then(responseBody),
  put: (url: string, body: {}) => axios.put(url, body).then(responseBody),
  del: (url: string) => axios.delete(url).then(responseBody),
  postForm: (url: string, file: Blob) => {
    let formData = new FormData();
    formData.append('File', file);

    return axios.post(url, formData, {
      headers: { 'Content-type': 'multipart/form-data' }
    }).then(responseBody);
  }
}

const Activities = {
  list: (params: URLSearchParams): Promise<IActivitiesEnvelope> => axios.get('/activities/list', {params: params}).then(responseBody),
  details: (id: string) => requests.get(`/activities/details/${id}`),
  create: (activity: IActivity) => requests.post(`/activities/create`, activity),
  edit: (activity: IActivity) => requests.put(`/activities/edit/${activity.id}`, activity),
  delete: (id: string) => requests.del(`/activities/delete/${id}`),
  attend: (id: string) => requests.post(`/activities/attend/${id}/attend`, {}),
  unattend: (id: string) => requests.del(`/activities/unattend/${id}/unattend`)
}

const User = {
  current: (): Promise<IUser> => requests.get(`/user/current-user`),
  login: (user: IUserFormValues): Promise<IUser> => requests.post(`/user/login`, user),
  register: (user: IUserFormValues): Promise<IUser> => requests.post(`/user/register`, user)
};

const Profiles = {
  get: (username: string): Promise<IProfile> => requests.get(`/profiles/details/${username}`),
  uploadPhoto: (photo: Blob): Promise<IPhoto> => requests.postForm(`/photos/add`, photo),
  setMainPhoto: (id: string) => requests.post(`photos/setmain/${id}/setmain`, {}),
  deletePhoto: (id: string) => requests.del(`photos/delete/${id}`),
  updateProfile: (profile: Partial<IProfile>) => requests.put(`/profiles/edit`, profile),
  follow: (username: string) => requests.post(`/profiles/follow/${username}/follow`, {}),
  unfollow: (username: string) => requests.del(`/profiles/unfollow/${username}/follow`),
  listFollowings: (username: string, predicate: string) => requests.get(`/profiles/getfollowings/${username}/follow?predicate=${predicate}`),
  listActivities: (username: string, predicate: string) => requests.get(`/profiles/getuseractivities/${username}/activities?predicate=${predicate}`)
};

const agent = {
  Activities,
  User,
  Profiles
};

export default agent;