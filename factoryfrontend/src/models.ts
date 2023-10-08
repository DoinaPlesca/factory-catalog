export class Box {
  boxId?: number ;
  boxName?: string;
  description ?: string;
  size ?: string;
  price ?: number ;
  imageUrl?: string;
}

export class ResponseDto<T> {
  responseData?: T;
  messageToClient?: string;
}
