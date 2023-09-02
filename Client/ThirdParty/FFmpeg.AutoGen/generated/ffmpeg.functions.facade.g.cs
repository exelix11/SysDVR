using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public static unsafe partial class ffmpeg
{
    /// <summary>Create an AVABufferSinkParams structure.</summary>
    [Obsolete()]
    public static AVABufferSinkParams* av_abuffersink_params_alloc() => vectors.av_abuffersink_params_alloc();
    
    /// <summary>Add an index entry into a sorted list. Update the entry if the list already contains it.</summary>
    /// <param name="timestamp">timestamp in the time base of the given stream</param>
    public static int av_add_index_entry(AVStream* @st, long @pos, long @timestamp, int @size, int @distance, int @flags) => vectors.av_add_index_entry(@st, @pos, @timestamp, @size, @distance, @flags);
    
    /// <summary>Add two rationals.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b+c</returns>
    public static AVRational av_add_q(AVRational @b, AVRational @c) => vectors.av_add_q(@b, @c);
    
    /// <summary>Add a value to a timestamp.</summary>
    /// <param name="ts_tb">Input timestamp time base</param>
    /// <param name="ts">Input timestamp</param>
    /// <param name="inc_tb">Time base of `inc`</param>
    /// <param name="inc">Value to be added</param>
    public static long av_add_stable(AVRational @ts_tb, long @ts, AVRational @inc_tb, long @inc) => vectors.av_add_stable(@ts_tb, @ts, @inc_tb, @inc);
    
    /// <summary>Read data and append it to the current content of the AVPacket. If pkt-&gt;size is 0 this is identical to av_get_packet. Note that this uses av_grow_packet and thus involves a realloc which is inefficient. Thus this function should only be used when there is no reasonable way to know (an upper bound of) the final size.</summary>
    /// <param name="s">associated IO context</param>
    /// <param name="pkt">packet</param>
    /// <param name="size">amount of data to read</param>
    /// <returns>&gt;0 (read size) if OK, AVERROR_xxx otherwise, previous data will not be lost even if an error occurs.</returns>
    public static int av_append_packet(AVIOContext* @s, AVPacket* @pkt, int @size) => vectors.av_append_packet(@s, @pkt, @size);
    
    /// <summary>Allocate an AVAudioFifo.</summary>
    /// <param name="sample_fmt">sample format</param>
    /// <param name="channels">number of channels</param>
    /// <param name="nb_samples">initial allocation size, in samples</param>
    /// <returns>newly allocated AVAudioFifo, or NULL on error</returns>
    public static AVAudioFifo* av_audio_fifo_alloc(AVSampleFormat @sample_fmt, int @channels, int @nb_samples) => vectors.av_audio_fifo_alloc(@sample_fmt, @channels, @nb_samples);
    
    /// <summary>Drain data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to drain</param>
    /// <param name="nb_samples">number of samples to drain</param>
    /// <returns>0 if OK, or negative AVERROR code on failure</returns>
    public static int av_audio_fifo_drain(AVAudioFifo* @af, int @nb_samples) => vectors.av_audio_fifo_drain(@af, @nb_samples);
    
    /// <summary>Free an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to free</param>
    public static void av_audio_fifo_free(AVAudioFifo* @af) => vectors.av_audio_fifo_free(@af);
    
    /// <summary>Peek data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to peek</param>
    /// <returns>number of samples actually peek, or negative AVERROR code on failure. The number of samples actually peek will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    public static int av_audio_fifo_peek(AVAudioFifo* @af, void** @data, int @nb_samples) => vectors.av_audio_fifo_peek(@af, @data, @nb_samples);
    
    /// <summary>Peek data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to peek</param>
    /// <param name="offset">offset from current read position</param>
    /// <returns>number of samples actually peek, or negative AVERROR code on failure. The number of samples actually peek will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    public static int av_audio_fifo_peek_at(AVAudioFifo* @af, void** @data, int @nb_samples, int @offset) => vectors.av_audio_fifo_peek_at(@af, @data, @nb_samples, @offset);
    
    /// <summary>Read data from an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to read from</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to read</param>
    /// <returns>number of samples actually read, or negative AVERROR code on failure. The number of samples actually read will not be greater than nb_samples, and will only be less than nb_samples if av_audio_fifo_size is less than nb_samples.</returns>
    public static int av_audio_fifo_read(AVAudioFifo* @af, void** @data, int @nb_samples) => vectors.av_audio_fifo_read(@af, @data, @nb_samples);
    
    /// <summary>Reallocate an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to reallocate</param>
    /// <param name="nb_samples">new allocation size, in samples</param>
    /// <returns>0 if OK, or negative AVERROR code on failure</returns>
    public static int av_audio_fifo_realloc(AVAudioFifo* @af, int @nb_samples) => vectors.av_audio_fifo_realloc(@af, @nb_samples);
    
    /// <summary>Reset the AVAudioFifo buffer.</summary>
    /// <param name="af">AVAudioFifo to reset</param>
    public static void av_audio_fifo_reset(AVAudioFifo* @af) => vectors.av_audio_fifo_reset(@af);
    
    /// <summary>Get the current number of samples in the AVAudioFifo available for reading.</summary>
    /// <param name="af">the AVAudioFifo to query</param>
    /// <returns>number of samples available for reading</returns>
    public static int av_audio_fifo_size(AVAudioFifo* @af) => vectors.av_audio_fifo_size(@af);
    
    /// <summary>Get the current number of samples in the AVAudioFifo available for writing.</summary>
    /// <param name="af">the AVAudioFifo to query</param>
    /// <returns>number of samples available for writing</returns>
    public static int av_audio_fifo_space(AVAudioFifo* @af) => vectors.av_audio_fifo_space(@af);
    
    /// <summary>Write data to an AVAudioFifo.</summary>
    /// <param name="af">AVAudioFifo to write to</param>
    /// <param name="data">audio data plane pointers</param>
    /// <param name="nb_samples">number of samples to write</param>
    /// <returns>number of samples actually written, or negative AVERROR code on failure. If successful, the number of samples actually written will always be nb_samples.</returns>
    public static int av_audio_fifo_write(AVAudioFifo* @af, void** @data, int @nb_samples) => vectors.av_audio_fifo_write(@af, @data, @nb_samples);
    
    /// <summary>Append a description of a channel layout to a bprint buffer.</summary>
    [Obsolete("use av_channel_layout_describe()")]
    public static void av_bprint_channel_layout(AVBPrint* @bp, int @nb_channels, ulong @channel_layout) => vectors.av_bprint_channel_layout(@bp, @nb_channels, @channel_layout);
    
    /// <summary>Allocate a context for a given bitstream filter. The caller must fill in the context parameters as described in the documentation and then call av_bsf_init() before sending any data to the filter.</summary>
    /// <param name="filter">the filter for which to allocate an instance.</param>
    /// <param name="ctx">a pointer into which the pointer to the newly-allocated context will be written. It must be freed with av_bsf_free() after the filtering is done.</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int av_bsf_alloc(AVBitStreamFilter* @filter, AVBSFContext** @ctx) => vectors.av_bsf_alloc(@filter, @ctx);
    
    /// <summary>Reset the internal bitstream filter state. Should be called e.g. when seeking.</summary>
    public static void av_bsf_flush(AVBSFContext* @ctx) => vectors.av_bsf_flush(@ctx);
    
    /// <summary>Free a bitstream filter context and everything associated with it; write NULL into the supplied pointer.</summary>
    public static void av_bsf_free(AVBSFContext** @ctx) => vectors.av_bsf_free(@ctx);
    
    /// <summary>Returns a bitstream filter with the specified name or NULL if no such bitstream filter exists.</summary>
    /// <returns>a bitstream filter with the specified name or NULL if no such bitstream filter exists.</returns>
    public static AVBitStreamFilter* av_bsf_get_by_name(string @name) => vectors.av_bsf_get_by_name(@name);
    
    /// <summary>Get the AVClass for AVBSFContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* av_bsf_get_class() => vectors.av_bsf_get_class();
    
    /// <summary>Get null/pass-through bitstream filter.</summary>
    /// <param name="bsf">Pointer to be set to new instance of pass-through bitstream filter</param>
    public static int av_bsf_get_null_filter(AVBSFContext** @bsf) => vectors.av_bsf_get_null_filter(@bsf);
    
    /// <summary>Prepare the filter for use, after all the parameters and options have been set.</summary>
    public static int av_bsf_init(AVBSFContext* @ctx) => vectors.av_bsf_init(@ctx);
    
    /// <summary>Iterate over all registered bitstream filters.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered bitstream filter or NULL when the iteration is finished</returns>
    public static AVBitStreamFilter* av_bsf_iterate(void** @opaque) => vectors.av_bsf_iterate(@opaque);
    
    /// <summary>Allocate empty list of bitstream filters. The list must be later freed by av_bsf_list_free() or finalized by av_bsf_list_finalize().</summary>
    /// <returns>Pointer to on success, NULL in case of failure</returns>
    public static AVBSFList* av_bsf_list_alloc() => vectors.av_bsf_list_alloc();
    
    /// <summary>Append bitstream filter to the list of bitstream filters.</summary>
    /// <param name="lst">List to append to</param>
    /// <param name="bsf">Filter context to be appended</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    public static int av_bsf_list_append(AVBSFList* @lst, AVBSFContext* @bsf) => vectors.av_bsf_list_append(@lst, @bsf);
    
    /// <summary>Construct new bitstream filter context given it&apos;s name and options and append it to the list of bitstream filters.</summary>
    /// <param name="lst">List to append to</param>
    /// <param name="bsf_name">Name of the bitstream filter</param>
    /// <param name="options">Options for the bitstream filter, can be set to NULL</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    public static int av_bsf_list_append2(AVBSFList* @lst, string @bsf_name, AVDictionary** @options) => vectors.av_bsf_list_append2(@lst, @bsf_name, @options);
    
    /// <summary>Finalize list of bitstream filters.</summary>
    /// <param name="lst">Filter list structure to be transformed</param>
    /// <param name="bsf">Pointer to be set to newly created structure representing the chain of bitstream filters</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    public static int av_bsf_list_finalize(AVBSFList** @lst, AVBSFContext** @bsf) => vectors.av_bsf_list_finalize(@lst, @bsf);
    
    /// <summary>Free list of bitstream filters.</summary>
    /// <param name="lst">Pointer to pointer returned by av_bsf_list_alloc()</param>
    public static void av_bsf_list_free(AVBSFList** @lst) => vectors.av_bsf_list_free(@lst);
    
    /// <summary>Parse string describing list of bitstream filters and create single AVBSFContext describing the whole chain of bitstream filters. Resulting AVBSFContext can be treated as any other AVBSFContext freshly allocated by av_bsf_alloc().</summary>
    /// <param name="str">String describing chain of bitstream filters in format `bsf1[=opt1=val1:opt2=val2][,bsf2]`</param>
    /// <param name="bsf">Pointer to be set to newly created structure representing the chain of bitstream filters</param>
    /// <returns>&gt;=0 on success, negative AVERROR in case of failure</returns>
    public static int av_bsf_list_parse_str(string @str, AVBSFContext** @bsf) => vectors.av_bsf_list_parse_str(@str, @bsf);
    
    /// <summary>Retrieve a filtered packet.</summary>
    /// <param name="pkt">this struct will be filled with the contents of the filtered packet. It is owned by the caller and must be freed using av_packet_unref() when it is no longer needed. This parameter should be &quot;clean&quot; (i.e. freshly allocated with av_packet_alloc() or unreffed with av_packet_unref()) when this function is called. If this function returns successfully, the contents of pkt will be completely overwritten by the returned data. On failure, pkt is not touched.</param>
    /// <returns>- 0 on success. - AVERROR(EAGAIN) if more packets need to be sent to the filter (using av_bsf_send_packet()) to get more output. - AVERROR_EOF if there will be no further output from the filter. - Another negative AVERROR value if an error occurs.</returns>
    public static int av_bsf_receive_packet(AVBSFContext* @ctx, AVPacket* @pkt) => vectors.av_bsf_receive_packet(@ctx, @pkt);
    
    /// <summary>Submit a packet for filtering.</summary>
    /// <param name="pkt">the packet to filter. The bitstream filter will take ownership of the packet and reset the contents of pkt. pkt is not touched if an error occurs. If pkt is empty (i.e. NULL, or pkt-&gt;data is NULL and pkt-&gt;side_data_elems zero), it signals the end of the stream (i.e. no more non-empty packets will be sent; sending more empty packets does nothing) and will cause the filter to output any packets it may have buffered internally.</param>
    /// <returns>- 0 on success. - AVERROR(EAGAIN) if packets need to be retrieved from the filter (using av_bsf_receive_packet()) before new input can be consumed. - Another negative AVERROR value if an error occurs.</returns>
    public static int av_bsf_send_packet(AVBSFContext* @ctx, AVPacket* @pkt) => vectors.av_bsf_send_packet(@ctx, @pkt);
    
    /// <summary>Allocate an AVBuffer of the given size using av_malloc().</summary>
    /// <returns>an AVBufferRef of given size or NULL when out of memory</returns>
    public static AVBufferRef* av_buffer_alloc(ulong @size) => vectors.av_buffer_alloc(@size);
    
    /// <summary>Same as av_buffer_alloc(), except the returned buffer will be initialized to zero.</summary>
    public static AVBufferRef* av_buffer_allocz(ulong @size) => vectors.av_buffer_allocz(@size);
    
    /// <summary>Create an AVBuffer from an existing array.</summary>
    /// <param name="data">data array</param>
    /// <param name="size">size of data in bytes</param>
    /// <param name="free">a callback for freeing this buffer&apos;s data</param>
    /// <param name="opaque">parameter to be got for processing or passed to free</param>
    /// <param name="flags">a combination of AV_BUFFER_FLAG_*</param>
    /// <returns>an AVBufferRef referring to data on success, NULL on failure.</returns>
    public static AVBufferRef* av_buffer_create(byte* @data, ulong @size, av_buffer_create_free_func @free, void* @opaque, int @flags) => vectors.av_buffer_create(@data, @size, @free, @opaque, @flags);
    
    /// <summary>Default free callback, which calls av_free() on the buffer data. This function is meant to be passed to av_buffer_create(), not called directly.</summary>
    public static void av_buffer_default_free(void* @opaque, byte* @data) => vectors.av_buffer_default_free(@opaque, @data);
    
    /// <summary>Returns the opaque parameter set by av_buffer_create.</summary>
    /// <returns>the opaque parameter set by av_buffer_create.</returns>
    public static void* av_buffer_get_opaque(AVBufferRef* @buf) => vectors.av_buffer_get_opaque(@buf);
    
    public static int av_buffer_get_ref_count(AVBufferRef* @buf) => vectors.av_buffer_get_ref_count(@buf);
    
    /// <summary>Returns 1 if the caller may write to the data referred to by buf (which is true if and only if buf is the only reference to the underlying AVBuffer). Return 0 otherwise. A positive answer is valid until av_buffer_ref() is called on buf.</summary>
    /// <returns>1 if the caller may write to the data referred to by buf (which is true if and only if buf is the only reference to the underlying AVBuffer). Return 0 otherwise. A positive answer is valid until av_buffer_ref() is called on buf.</returns>
    public static int av_buffer_is_writable(AVBufferRef* @buf) => vectors.av_buffer_is_writable(@buf);
    
    /// <summary>Create a writable reference from a given buffer reference, avoiding data copy if possible.</summary>
    /// <param name="buf">buffer reference to make writable. On success, buf is either left untouched, or it is unreferenced and a new writable AVBufferRef is written in its place. On failure, buf is left untouched.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    public static int av_buffer_make_writable(AVBufferRef** @buf) => vectors.av_buffer_make_writable(@buf);
    
    /// <summary>Query the original opaque parameter of an allocated buffer in the pool.</summary>
    /// <param name="ref">a buffer reference to a buffer returned by av_buffer_pool_get.</param>
    /// <returns>the opaque parameter set by the buffer allocator function of the buffer pool.</returns>
    public static void* av_buffer_pool_buffer_get_opaque(AVBufferRef* @ref) => vectors.av_buffer_pool_buffer_get_opaque(@ref);
    
    /// <summary>Allocate a new AVBuffer, reusing an old buffer from the pool when available. This function may be called simultaneously from multiple threads.</summary>
    /// <returns>a reference to the new buffer on success, NULL on error.</returns>
    public static AVBufferRef* av_buffer_pool_get(AVBufferPool* @pool) => vectors.av_buffer_pool_get(@pool);
    
    /// <summary>Allocate and initialize a buffer pool.</summary>
    /// <param name="size">size of each buffer in this pool</param>
    /// <param name="alloc">a function that will be used to allocate new buffers when the pool is empty. May be NULL, then the default allocator will be used (av_buffer_alloc()).</param>
    /// <returns>newly created buffer pool on success, NULL on error.</returns>
    public static AVBufferPool* av_buffer_pool_init(ulong @size, av_buffer_pool_init_alloc_func @alloc) => vectors.av_buffer_pool_init(@size, @alloc);
    
    /// <summary>Allocate and initialize a buffer pool with a more complex allocator.</summary>
    /// <param name="size">size of each buffer in this pool</param>
    /// <param name="opaque">arbitrary user data used by the allocator</param>
    /// <param name="alloc">a function that will be used to allocate new buffers when the pool is empty. May be NULL, then the default allocator will be used (av_buffer_alloc()).</param>
    /// <param name="pool_free">a function that will be called immediately before the pool is freed. I.e. after av_buffer_pool_uninit() is called by the caller and all the frames are returned to the pool and freed. It is intended to uninitialize the user opaque data. May be NULL.</param>
    /// <returns>newly created buffer pool on success, NULL on error.</returns>
    public static AVBufferPool* av_buffer_pool_init2(ulong @size, void* @opaque, av_buffer_pool_init2_alloc_func @alloc, av_buffer_pool_init2_pool_free_func @pool_free) => vectors.av_buffer_pool_init2(@size, @opaque, @alloc, @pool_free);
    
    /// <summary>Mark the pool as being available for freeing. It will actually be freed only once all the allocated buffers associated with the pool are released. Thus it is safe to call this function while some of the allocated buffers are still in use.</summary>
    /// <param name="pool">pointer to the pool to be freed. It will be set to NULL.</param>
    public static void av_buffer_pool_uninit(AVBufferPool** @pool) => vectors.av_buffer_pool_uninit(@pool);
    
    /// <summary>Reallocate a given buffer.</summary>
    /// <param name="buf">a buffer reference to reallocate. On success, buf will be unreferenced and a new reference with the required size will be written in its place. On failure buf will be left untouched. *buf may be NULL, then a new buffer is allocated.</param>
    /// <param name="size">required new buffer size.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    public static int av_buffer_realloc(AVBufferRef** @buf, ulong @size) => vectors.av_buffer_realloc(@buf, @size);
    
    /// <summary>Create a new reference to an AVBuffer.</summary>
    /// <returns>a new AVBufferRef referring to the same AVBuffer as buf or NULL on failure.</returns>
    public static AVBufferRef* av_buffer_ref(AVBufferRef* @buf) => vectors.av_buffer_ref(@buf);
    
    /// <summary>Ensure dst refers to the same data as src.</summary>
    /// <param name="dst">Pointer to either a valid buffer reference or NULL. On success, this will point to a buffer reference equivalent to src. On failure, dst will be left untouched.</param>
    /// <param name="src">A buffer reference to replace dst with. May be NULL, then this function is equivalent to av_buffer_unref(dst).</param>
    /// <returns>0 on success AVERROR(ENOMEM) on memory allocation failure.</returns>
    public static int av_buffer_replace(AVBufferRef** @dst, AVBufferRef* @src) => vectors.av_buffer_replace(@dst, @src);
    
    /// <summary>Free a given reference and automatically free the buffer if there are no more references to it.</summary>
    /// <param name="buf">the reference to be freed. The pointer is set to NULL on return.</param>
    public static void av_buffer_unref(AVBufferRef** @buf) => vectors.av_buffer_unref(@buf);
    
    public static int av_buffersink_get_ch_layout(AVFilterContext* @ctx, AVChannelLayout* @ch_layout) => vectors.av_buffersink_get_ch_layout(@ctx, @ch_layout);
    
    [Obsolete()]
    public static ulong av_buffersink_get_channel_layout(AVFilterContext* @ctx) => vectors.av_buffersink_get_channel_layout(@ctx);
    
    public static int av_buffersink_get_channels(AVFilterContext* @ctx) => vectors.av_buffersink_get_channels(@ctx);
    
    public static int av_buffersink_get_format(AVFilterContext* @ctx) => vectors.av_buffersink_get_format(@ctx);
    
    /// <summary>Get a frame with filtered data from sink and put it in frame.</summary>
    /// <param name="ctx">pointer to a context of a buffersink or abuffersink AVFilter.</param>
    /// <param name="frame">pointer to an allocated frame that will be filled with data. The data must be freed using av_frame_unref() / av_frame_free()</param>
    /// <returns>- &gt;= 0 if a frame was successfully returned. - AVERROR(EAGAIN) if no frames are available at this point; more input frames must be added to the filtergraph to get more output. - AVERROR_EOF if there will be no more output frames on this sink. - A different negative AVERROR code in other failure cases.</returns>
    public static int av_buffersink_get_frame(AVFilterContext* @ctx, AVFrame* @frame) => vectors.av_buffersink_get_frame(@ctx, @frame);
    
    /// <summary>Get a frame with filtered data from sink and put it in frame.</summary>
    /// <param name="ctx">pointer to a buffersink or abuffersink filter context.</param>
    /// <param name="frame">pointer to an allocated frame that will be filled with data. The data must be freed using av_frame_unref() / av_frame_free()</param>
    /// <param name="flags">a combination of AV_BUFFERSINK_FLAG_* flags</param>
    /// <returns>&gt;= 0 in for success, a negative AVERROR code for failure.</returns>
    public static int av_buffersink_get_frame_flags(AVFilterContext* @ctx, AVFrame* @frame, int @flags) => vectors.av_buffersink_get_frame_flags(@ctx, @frame, @flags);
    
    public static AVRational av_buffersink_get_frame_rate(AVFilterContext* @ctx) => vectors.av_buffersink_get_frame_rate(@ctx);
    
    public static int av_buffersink_get_h(AVFilterContext* @ctx) => vectors.av_buffersink_get_h(@ctx);
    
    public static AVBufferRef* av_buffersink_get_hw_frames_ctx(AVFilterContext* @ctx) => vectors.av_buffersink_get_hw_frames_ctx(@ctx);
    
    public static AVRational av_buffersink_get_sample_aspect_ratio(AVFilterContext* @ctx) => vectors.av_buffersink_get_sample_aspect_ratio(@ctx);
    
    public static int av_buffersink_get_sample_rate(AVFilterContext* @ctx) => vectors.av_buffersink_get_sample_rate(@ctx);
    
    /// <summary>Same as av_buffersink_get_frame(), but with the ability to specify the number of samples read. This function is less efficient than av_buffersink_get_frame(), because it copies the data around.</summary>
    /// <param name="ctx">pointer to a context of the abuffersink AVFilter.</param>
    /// <param name="frame">pointer to an allocated frame that will be filled with data. The data must be freed using av_frame_unref() / av_frame_free() frame will contain exactly nb_samples audio samples, except at the end of stream, when it can contain less than nb_samples.</param>
    /// <returns>The return codes have the same meaning as for av_buffersink_get_frame().</returns>
    public static int av_buffersink_get_samples(AVFilterContext* @ctx, AVFrame* @frame, int @nb_samples) => vectors.av_buffersink_get_samples(@ctx, @frame, @nb_samples);
    
    public static AVRational av_buffersink_get_time_base(AVFilterContext* @ctx) => vectors.av_buffersink_get_time_base(@ctx);
    
    /// <summary>Get the properties of the stream @{</summary>
    public static AVMediaType av_buffersink_get_type(AVFilterContext* @ctx) => vectors.av_buffersink_get_type(@ctx);
    
    public static int av_buffersink_get_w(AVFilterContext* @ctx) => vectors.av_buffersink_get_w(@ctx);
    
    /// <summary>Create an AVBufferSinkParams structure.</summary>
    [Obsolete()]
    public static AVBufferSinkParams* av_buffersink_params_alloc() => vectors.av_buffersink_params_alloc();
    
    /// <summary>Set the frame size for an audio buffer sink.</summary>
    public static void av_buffersink_set_frame_size(AVFilterContext* @ctx, uint @frame_size) => vectors.av_buffersink_set_frame_size(@ctx, @frame_size);
    
    /// <summary>Add a frame to the buffer source.</summary>
    /// <param name="ctx">an instance of the buffersrc filter</param>
    /// <param name="frame">frame to be added. If the frame is reference counted, this function will take ownership of the reference(s) and reset the frame. Otherwise the frame data will be copied. If this function returns an error, the input frame is not touched.</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    public static int av_buffersrc_add_frame(AVFilterContext* @ctx, AVFrame* @frame) => vectors.av_buffersrc_add_frame(@ctx, @frame);
    
    /// <summary>Add a frame to the buffer source.</summary>
    /// <param name="buffer_src">pointer to a buffer source context</param>
    /// <param name="frame">a frame, or NULL to mark EOF</param>
    /// <param name="flags">a combination of AV_BUFFERSRC_FLAG_*</param>
    /// <returns>&gt;= 0 in case of success, a negative AVERROR code in case of failure</returns>
    public static int av_buffersrc_add_frame_flags(AVFilterContext* @buffer_src, AVFrame* @frame, int @flags) => vectors.av_buffersrc_add_frame_flags(@buffer_src, @frame, @flags);
    
    /// <summary>Close the buffer source after EOF.</summary>
    public static int av_buffersrc_close(AVFilterContext* @ctx, long @pts, uint @flags) => vectors.av_buffersrc_close(@ctx, @pts, @flags);
    
    /// <summary>Get the number of failed requests.</summary>
    public static uint av_buffersrc_get_nb_failed_requests(AVFilterContext* @buffer_src) => vectors.av_buffersrc_get_nb_failed_requests(@buffer_src);
    
    /// <summary>Allocate a new AVBufferSrcParameters instance. It should be freed by the caller with av_free().</summary>
    public static AVBufferSrcParameters* av_buffersrc_parameters_alloc() => vectors.av_buffersrc_parameters_alloc();
    
    /// <summary>Initialize the buffersrc or abuffersrc filter with the provided parameters. This function may be called multiple times, the later calls override the previous ones. Some of the parameters may also be set through AVOptions, then whatever method is used last takes precedence.</summary>
    /// <param name="ctx">an instance of the buffersrc or abuffersrc filter</param>
    /// <param name="param">the stream parameters. The frames later passed to this filter must conform to those parameters. All the allocated fields in param remain owned by the caller, libavfilter will make internal copies or references when necessary.</param>
    /// <returns>0 on success, a negative AVERROR code on failure.</returns>
    public static int av_buffersrc_parameters_set(AVFilterContext* @ctx, AVBufferSrcParameters* @param) => vectors.av_buffersrc_parameters_set(@ctx, @param);
    
    /// <summary>Add a frame to the buffer source.</summary>
    /// <param name="ctx">an instance of the buffersrc filter</param>
    /// <param name="frame">frame to be added. If the frame is reference counted, this function will make a new reference to it. Otherwise the frame data will be copied.</param>
    /// <returns>0 on success, a negative AVERROR on error</returns>
    public static int av_buffersrc_write_frame(AVFilterContext* @ctx, AVFrame* @frame) => vectors.av_buffersrc_write_frame(@ctx, @frame);
    
    /// <summary>Allocate a memory block for an array with av_mallocz().</summary>
    /// <param name="nmemb">Number of elements</param>
    /// <param name="size">Size of the single element</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    public static void* av_calloc(ulong @nmemb, ulong @size) => vectors.av_calloc(@nmemb, @size);
    
    /// <summary>Get a human readable string describing a given channel.</summary>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    public static int av_channel_description(byte* @buf, ulong @buf_size, AVChannel @channel) => vectors.av_channel_description(@buf, @buf_size, @channel);
    
    /// <summary>bprint variant of av_channel_description().</summary>
    public static void av_channel_description_bprint(AVBPrint* @bp, AVChannel @channel_id) => vectors.av_channel_description_bprint(@bp, @channel_id);
    
    /// <summary>This is the inverse function of av_channel_name().</summary>
    /// <returns>the channel with the given name AV_CHAN_NONE when name does not identify a known channel</returns>
    public static AVChannel av_channel_from_string(string @name) => vectors.av_channel_from_string(@name);
    
    /// <summary>Get the channel with the given index in a channel layout.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>channel with the index idx in channel_layout on success or AV_CHAN_NONE on failure (if idx is not valid or the channel order is unspecified)</returns>
    public static AVChannel av_channel_layout_channel_from_index(AVChannelLayout* @channel_layout, uint @idx) => vectors.av_channel_layout_channel_from_index(@channel_layout, @idx);
    
    /// <summary>Get a channel described by the given string.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>a channel described by the given string in channel_layout on success or AV_CHAN_NONE on failure (if the string is not valid or the channel order is unspecified)</returns>
    public static AVChannel av_channel_layout_channel_from_string(AVChannelLayout* @channel_layout, string @name) => vectors.av_channel_layout_channel_from_string(@channel_layout, @name);
    
    /// <summary>Check whether a channel layout is valid, i.e. can possibly describe audio data.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>1 if channel_layout is valid, 0 otherwise.</returns>
    public static int av_channel_layout_check(AVChannelLayout* @channel_layout) => vectors.av_channel_layout_check(@channel_layout);
    
    /// <summary>Check whether two channel layouts are semantically the same, i.e. the same channels are present on the same positions in both.</summary>
    /// <param name="chl">input channel layout</param>
    /// <param name="chl1">input channel layout</param>
    /// <returns>0 if chl and chl1 are equal, 1 if they are not equal. A negative AVERROR code if one or both are invalid.</returns>
    public static int av_channel_layout_compare(AVChannelLayout* @chl, AVChannelLayout* @chl1) => vectors.av_channel_layout_compare(@chl, @chl1);
    
    /// <summary>Make a copy of a channel layout. This differs from just assigning src to dst in that it allocates and copies the map for AV_CHANNEL_ORDER_CUSTOM.</summary>
    /// <param name="dst">destination channel layout</param>
    /// <param name="src">source channel layout</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    public static int av_channel_layout_copy(AVChannelLayout* @dst, AVChannelLayout* @src) => vectors.av_channel_layout_copy(@dst, @src);
    
    /// <summary>Get the default channel layout for a given number of channels.</summary>
    /// <param name="nb_channels">number of channels</param>
    public static void av_channel_layout_default(AVChannelLayout* @ch_layout, int @nb_channels) => vectors.av_channel_layout_default(@ch_layout, @nb_channels);
    
    /// <summary>Get a human-readable string describing the channel layout properties. The string will be in the same format that is accepted by av_channel_layout_from_string(), allowing to rebuild the same channel layout, except for opaque pointers.</summary>
    /// <param name="channel_layout">channel layout to be described</param>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    public static int av_channel_layout_describe(AVChannelLayout* @channel_layout, byte* @buf, ulong @buf_size) => vectors.av_channel_layout_describe(@channel_layout, @buf, @buf_size);
    
    /// <summary>bprint variant of av_channel_layout_describe().</summary>
    /// <returns>0 on success, or a negative AVERROR value on failure.</returns>
    public static int av_channel_layout_describe_bprint(AVChannelLayout* @channel_layout, AVBPrint* @bp) => vectors.av_channel_layout_describe_bprint(@channel_layout, @bp);
    
    /// <summary>Get the channel with the given index in channel_layout.</summary>
    [Obsolete("use av_channel_layout_channel_from_index()")]
    public static ulong av_channel_layout_extract_channel(ulong @channel_layout, int @index) => vectors.av_channel_layout_extract_channel(@channel_layout, @index);
    
    /// <summary>Initialize a native channel layout from a bitmask indicating which channels are present.</summary>
    /// <param name="channel_layout">the layout structure to be initialized</param>
    /// <param name="mask">bitmask describing the channel layout</param>
    /// <returns>0 on success AVERROR(EINVAL) for invalid mask values</returns>
    public static int av_channel_layout_from_mask(AVChannelLayout* @channel_layout, ulong @mask) => vectors.av_channel_layout_from_mask(@channel_layout, @mask);
    
    /// <summary>Initialize a channel layout from a given string description. The input string can be represented by: - the formal channel layout name (returned by av_channel_layout_describe()) - single or multiple channel names (returned by av_channel_name(), eg. &quot;FL&quot;, or concatenated with &quot;+&quot;, each optionally containing a custom name after a &quot;&quot;, eg. &quot;FL+FR+LFE&quot;) - a decimal or hexadecimal value of a native channel layout (eg. &quot;4&quot; or &quot;0x4&quot;) - the number of channels with default layout (eg. &quot;4c&quot;) - the number of unordered channels (eg. &quot;4C&quot; or &quot;4 channels&quot;) - the ambisonic order followed by optional non-diegetic channels (eg. &quot;ambisonic 2+stereo&quot;)</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="str">string describing the channel layout</param>
    /// <returns>0 channel layout was detected, AVERROR_INVALIDATATA otherwise</returns>
    public static int av_channel_layout_from_string(AVChannelLayout* @channel_layout, string @str) => vectors.av_channel_layout_from_string(@channel_layout, @str);
    
    /// <summary>Get the index of a given channel in a channel layout. In case multiple channels are found, only the first match will be returned.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>index of channel in channel_layout on success or a negative number if channel is not present in channel_layout.</returns>
    public static int av_channel_layout_index_from_channel(AVChannelLayout* @channel_layout, AVChannel @channel) => vectors.av_channel_layout_index_from_channel(@channel_layout, @channel);
    
    /// <summary>Get the index in a channel layout of a channel described by the given string. In case multiple channels are found, only the first match will be returned.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <returns>a channel index described by the given string, or a negative AVERROR value.</returns>
    public static int av_channel_layout_index_from_string(AVChannelLayout* @channel_layout, string @name) => vectors.av_channel_layout_index_from_string(@channel_layout, @name);
    
    /// <summary>Iterate over all standard channel layouts.</summary>
    /// <param name="opaque">a pointer where libavutil will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the standard channel layout or NULL when the iteration is finished</returns>
    public static AVChannelLayout* av_channel_layout_standard(void** @opaque) => vectors.av_channel_layout_standard(@opaque);
    
    /// <summary>Find out what channels from a given set are present in a channel layout, without regard for their positions.</summary>
    /// <param name="channel_layout">input channel layout</param>
    /// <param name="mask">a combination of AV_CH_* representing a set of channels</param>
    /// <returns>a bitfield representing all the channels from mask that are present in channel_layout</returns>
    public static ulong av_channel_layout_subset(AVChannelLayout* @channel_layout, ulong @mask) => vectors.av_channel_layout_subset(@channel_layout, @mask);
    
    /// <summary>Free any allocated data in the channel layout and reset the channel count to 0.</summary>
    /// <param name="channel_layout">the layout structure to be uninitialized</param>
    public static void av_channel_layout_uninit(AVChannelLayout* @channel_layout) => vectors.av_channel_layout_uninit(@channel_layout);
    
    /// <summary>Get a human readable string in an abbreviated form describing a given channel. This is the inverse function of av_channel_from_string().</summary>
    /// <param name="buf">pre-allocated buffer where to put the generated string</param>
    /// <param name="buf_size">size in bytes of the buffer.</param>
    /// <returns>amount of bytes needed to hold the output string, or a negative AVERROR on failure. If the returned value is bigger than buf_size, then the string was truncated.</returns>
    public static int av_channel_name(byte* @buf, ulong @buf_size, AVChannel @channel) => vectors.av_channel_name(@buf, @buf_size, @channel);
    
    /// <summary>bprint variant of av_channel_name().</summary>
    public static void av_channel_name_bprint(AVBPrint* @bp, AVChannel @channel_id) => vectors.av_channel_name_bprint(@bp, @channel_id);
    
    /// <summary>Returns the AVChromaLocation value for name or an AVError if not found.</summary>
    /// <returns>the AVChromaLocation value for name or an AVError if not found.</returns>
    public static int av_chroma_location_from_name(string @name) => vectors.av_chroma_location_from_name(@name);
    
    /// <summary>Returns the name for provided chroma location or NULL if unknown.</summary>
    /// <returns>the name for provided chroma location or NULL if unknown.</returns>
    public static string av_chroma_location_name(AVChromaLocation @location) => vectors.av_chroma_location_name(@location);
    
    /// <summary>Get the AVCodecID for the given codec tag tag. If no codec id is found returns AV_CODEC_ID_NONE.</summary>
    /// <param name="tags">list of supported codec_id-codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="tag">codec tag to match to a codec ID</param>
    public static AVCodecID av_codec_get_id(AVCodecTag** @tags, uint @tag) => vectors.av_codec_get_id(@tags, @tag);
    
    /// <summary>Get the codec tag for the given codec id id. If no codec tag is found returns 0.</summary>
    /// <param name="tags">list of supported codec_id-codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="id">codec ID to match to a codec tag</param>
    public static uint av_codec_get_tag(AVCodecTag** @tags, AVCodecID @id) => vectors.av_codec_get_tag(@tags, @id);
    
    /// <summary>Get the codec tag for the given codec id.</summary>
    /// <param name="tags">list of supported codec_id - codec_tag pairs, as stored in AVInputFormat.codec_tag and AVOutputFormat.codec_tag</param>
    /// <param name="id">codec id that should be searched for in the list</param>
    /// <param name="tag">A pointer to the found tag</param>
    /// <returns>0 if id was not found in tags, &gt; 0 if it was found</returns>
    public static int av_codec_get_tag2(AVCodecTag** @tags, AVCodecID @id, uint* @tag) => vectors.av_codec_get_tag2(@tags, @id, @tag);
    
    /// <summary>Returns a non-zero number if codec is a decoder, zero otherwise</summary>
    /// <returns>a non-zero number if codec is a decoder, zero otherwise</returns>
    public static int av_codec_is_decoder(AVCodec* @codec) => vectors.av_codec_is_decoder(@codec);
    
    /// <summary>Returns a non-zero number if codec is an encoder, zero otherwise</summary>
    /// <returns>a non-zero number if codec is an encoder, zero otherwise</returns>
    public static int av_codec_is_encoder(AVCodec* @codec) => vectors.av_codec_is_encoder(@codec);
    
    /// <summary>Iterate over all registered codecs.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered codec or NULL when the iteration is finished</returns>
    public static AVCodec* av_codec_iterate(void** @opaque) => vectors.av_codec_iterate(@opaque);
    
    /// <summary>Returns the AVColorPrimaries value for name or an AVError if not found.</summary>
    /// <returns>the AVColorPrimaries value for name or an AVError if not found.</returns>
    public static int av_color_primaries_from_name(string @name) => vectors.av_color_primaries_from_name(@name);
    
    /// <summary>Returns the name for provided color primaries or NULL if unknown.</summary>
    /// <returns>the name for provided color primaries or NULL if unknown.</returns>
    public static string av_color_primaries_name(AVColorPrimaries @primaries) => vectors.av_color_primaries_name(@primaries);
    
    /// <summary>Returns the AVColorRange value for name or an AVError if not found.</summary>
    /// <returns>the AVColorRange value for name or an AVError if not found.</returns>
    public static int av_color_range_from_name(string @name) => vectors.av_color_range_from_name(@name);
    
    /// <summary>Returns the name for provided color range or NULL if unknown.</summary>
    /// <returns>the name for provided color range or NULL if unknown.</returns>
    public static string av_color_range_name(AVColorRange @range) => vectors.av_color_range_name(@range);
    
    /// <summary>Returns the AVColorSpace value for name or an AVError if not found.</summary>
    /// <returns>the AVColorSpace value for name or an AVError if not found.</returns>
    public static int av_color_space_from_name(string @name) => vectors.av_color_space_from_name(@name);
    
    /// <summary>Returns the name for provided color space or NULL if unknown.</summary>
    /// <returns>the name for provided color space or NULL if unknown.</returns>
    public static string av_color_space_name(AVColorSpace @space) => vectors.av_color_space_name(@space);
    
    /// <summary>Returns the AVColorTransferCharacteristic value for name or an AVError if not found.</summary>
    /// <returns>the AVColorTransferCharacteristic value for name or an AVError if not found.</returns>
    public static int av_color_transfer_from_name(string @name) => vectors.av_color_transfer_from_name(@name);
    
    /// <summary>Returns the name for provided color transfer or NULL if unknown.</summary>
    /// <returns>the name for provided color transfer or NULL if unknown.</returns>
    public static string av_color_transfer_name(AVColorTransferCharacteristic @transfer) => vectors.av_color_transfer_name(@transfer);
    
    /// <summary>Compare the remainders of two integer operands divided by a common divisor.</summary>
    /// <param name="mod">Divisor; must be a power of 2</param>
    /// <returns>- a negative value if `a % mod &lt; b % mod` - a positive value if `a % mod &gt; b % mod` - zero             if `a % mod == b % mod`</returns>
    public static long av_compare_mod(ulong @a, ulong @b, ulong @mod) => vectors.av_compare_mod(@a, @b, @mod);
    
    /// <summary>Compare two timestamps each in its own time base.</summary>
    /// <returns>One of the following values: - -1 if `ts_a` is before `ts_b` - 1 if `ts_a` is after `ts_b` - 0 if they represent the same position</returns>
    public static int av_compare_ts(long @ts_a, AVRational @tb_a, long @ts_b, AVRational @tb_b) => vectors.av_compare_ts(@ts_a, @tb_a, @ts_b, @tb_b);
    
    /// <summary>Allocate an AVContentLightMetadata structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVContentLightMetadata filled with default values or NULL on failure.</returns>
    public static AVContentLightMetadata* av_content_light_metadata_alloc(ulong* @size) => vectors.av_content_light_metadata_alloc(@size);
    
    /// <summary>Allocate a complete AVContentLightMetadata and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVContentLightMetadata structure to be filled by caller.</returns>
    public static AVContentLightMetadata* av_content_light_metadata_create_side_data(AVFrame* @frame) => vectors.av_content_light_metadata_create_side_data(@frame);
    
    /// <summary>Allocate a CPB properties structure and initialize its fields to default values.</summary>
    /// <param name="size">if non-NULL, the size of the allocated struct will be written here. This is useful for embedding it in side data.</param>
    /// <returns>the newly allocated struct or NULL on failure</returns>
    public static AVCPBProperties* av_cpb_properties_alloc(ulong* @size) => vectors.av_cpb_properties_alloc(@size);
    
    /// <summary>Returns the number of logical CPU cores present.</summary>
    /// <returns>the number of logical CPU cores present.</returns>
    public static int av_cpu_count() => vectors.av_cpu_count();
    
    /// <summary>Overrides cpu count detection and forces the specified count. Count &lt; 1 disables forcing of specific count.</summary>
    public static void av_cpu_force_count(int @count) => vectors.av_cpu_force_count(@count);
    
    /// <summary>Get the maximum data alignment that may be required by FFmpeg.</summary>
    public static ulong av_cpu_max_align() => vectors.av_cpu_max_align();
    
    /// <summary>Convert a double precision floating point number to a rational.</summary>
    /// <param name="d">`double` to convert</param>
    /// <param name="max">Maximum allowed numerator and denominator</param>
    /// <returns>`d` in AVRational form</returns>
    public static AVRational av_d2q(double @d, int @max) => vectors.av_d2q(@d, @max);
    
    /// <summary>Allocate an AVD3D11VAContext.</summary>
    /// <returns>Newly-allocated AVD3D11VAContext or NULL on failure.</returns>
    public static AVD3D11VAContext* av_d3d11va_alloc_context() => vectors.av_d3d11va_alloc_context();
    
    public static AVClassCategory av_default_get_category(void* @ptr) => vectors.av_default_get_category(@ptr);
    
    /// <summary>Return the context name</summary>
    /// <param name="ctx">The AVClass context</param>
    /// <returns>The AVClass class_name</returns>
    public static string av_default_item_name(void* @ctx) => vectors.av_default_item_name(@ctx);
    
    /// <summary>Iterate over all registered demuxers.</summary>
    /// <param name="opaque">a pointer where libavformat will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered demuxer or NULL when the iteration is finished</returns>
    public static AVInputFormat* av_demuxer_iterate(void** @opaque) => vectors.av_demuxer_iterate(@opaque);
    
    /// <summary>Copy entries from one AVDictionary struct into another.</summary>
    /// <param name="dst">pointer to a pointer to a AVDictionary struct. If *dst is NULL, this function will allocate a struct for you and put it in *dst</param>
    /// <param name="src">pointer to source AVDictionary struct</param>
    /// <param name="flags">flags to use when setting entries in *dst</param>
    /// <returns>0 on success, negative AVERROR code on failure. If dst was allocated by this function, callers should free the associated memory.</returns>
    public static int av_dict_copy(AVDictionary** @dst, AVDictionary* @src, int @flags) => vectors.av_dict_copy(@dst, @src, @flags);
    
    /// <summary>Get number of entries in dictionary.</summary>
    /// <param name="m">dictionary</param>
    /// <returns>number of entries in dictionary</returns>
    public static int av_dict_count(AVDictionary* @m) => vectors.av_dict_count(@m);
    
    /// <summary>Free all the memory allocated for an AVDictionary struct and all keys and values.</summary>
    public static void av_dict_free(AVDictionary** @m) => vectors.av_dict_free(@m);
    
    /// <summary>Get a dictionary entry with matching key.</summary>
    /// <param name="key">matching key</param>
    /// <param name="prev">Set to the previous matching element to find the next. If set to NULL the first matching element is returned.</param>
    /// <param name="flags">a collection of AV_DICT_* flags controlling how the entry is retrieved</param>
    /// <returns>found entry or NULL in case no matching entry was found in the dictionary</returns>
    public static AVDictionaryEntry* av_dict_get(AVDictionary* @m, string @key, AVDictionaryEntry* @prev, int @flags) => vectors.av_dict_get(@m, @key, @prev, @flags);
    
    /// <summary>Get dictionary entries as a string.</summary>
    /// <param name="m">dictionary</param>
    /// <param name="buffer">Pointer to buffer that will be allocated with string containg entries. Buffer must be freed by the caller when is no longer needed.</param>
    /// <param name="key_val_sep">character used to separate key from value</param>
    /// <param name="pairs_sep">character used to separate two pairs from each other</param>
    /// <returns>&gt;= 0 on success, negative on error</returns>
    public static int av_dict_get_string(AVDictionary* @m, byte** @buffer, byte @key_val_sep, byte @pairs_sep) => vectors.av_dict_get_string(@m, @buffer, @key_val_sep, @pairs_sep);
    
    /// <summary>Parse the key/value pairs list and add the parsed entries to a dictionary.</summary>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other</param>
    /// <param name="flags">flags to use when adding to dictionary. AV_DICT_DONT_STRDUP_KEY and AV_DICT_DONT_STRDUP_VAL are ignored since the key/value tokens will always be duplicated.</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    public static int av_dict_parse_string(AVDictionary** @pm, string @str, string @key_val_sep, string @pairs_sep, int @flags) => vectors.av_dict_parse_string(@pm, @str, @key_val_sep, @pairs_sep, @flags);
    
    /// <summary>Set the given entry in *pm, overwriting an existing entry.</summary>
    /// <param name="pm">pointer to a pointer to a dictionary struct. If *pm is NULL a dictionary struct is allocated and put in *pm.</param>
    /// <param name="key">entry key to add to *pm (will either be av_strduped or added as a new key depending on flags)</param>
    /// <param name="value">entry value to add to *pm (will be av_strduped or added as a new key depending on flags). Passing a NULL value will cause an existing entry to be deleted.</param>
    /// <returns>&gt;= 0 on success otherwise an error code &lt; 0</returns>
    public static int av_dict_set(AVDictionary** @pm, string @key, string @value, int @flags) => vectors.av_dict_set(@pm, @key, @value, @flags);
    
    /// <summary>Convenience wrapper for av_dict_set that converts the value to a string and stores it.</summary>
    public static int av_dict_set_int(AVDictionary** @pm, string @key, long @value, int @flags) => vectors.av_dict_set_int(@pm, @key, @value, @flags);
    
    /// <summary>Returns The AV_DISPOSITION_* flag corresponding to disp or a negative error code if disp does not correspond to a known stream disposition.</summary>
    /// <returns>The AV_DISPOSITION_* flag corresponding to disp or a negative error code if disp does not correspond to a known stream disposition.</returns>
    public static int av_disposition_from_string(string @disp) => vectors.av_disposition_from_string(@disp);
    
    /// <summary>Returns The string description corresponding to the lowest set bit in disposition. NULL when the lowest set bit does not correspond to a known disposition or when disposition is 0.</summary>
    /// <param name="disposition">a combination of AV_DISPOSITION_* values</param>
    /// <returns>The string description corresponding to the lowest set bit in disposition. NULL when the lowest set bit does not correspond to a known disposition or when disposition is 0.</returns>
    public static string av_disposition_to_string(int @disposition) => vectors.av_disposition_to_string(@disposition);
    
    /// <summary>Divide one rational by another.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b/c</returns>
    public static AVRational av_div_q(AVRational @b, AVRational @c) => vectors.av_div_q(@b, @c);
    
    /// <summary>Print detailed information about the input or output format, such as duration, bitrate, streams, container, programs, metadata, side data, codec and time base.</summary>
    /// <param name="ic">the context to analyze</param>
    /// <param name="index">index of the stream to dump information about</param>
    /// <param name="url">the URL to print, such as source or destination file</param>
    /// <param name="is_output">Select whether the specified context is an input(0) or output(1)</param>
    public static void av_dump_format(AVFormatContext* @ic, int @index, string @url, int @is_output) => vectors.av_dump_format(@ic, @index, @url, @is_output);
    
    /// <summary>Allocate an AVDynamicHDRPlus structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVDynamicHDRPlus filled with default values or NULL on failure.</returns>
    public static AVDynamicHDRPlus* av_dynamic_hdr_plus_alloc(ulong* @size) => vectors.av_dynamic_hdr_plus_alloc(@size);
    
    /// <summary>Allocate a complete AVDynamicHDRPlus and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVDynamicHDRPlus structure to be filled by caller or NULL on failure.</returns>
    public static AVDynamicHDRPlus* av_dynamic_hdr_plus_create_side_data(AVFrame* @frame) => vectors.av_dynamic_hdr_plus_create_side_data(@frame);
    
    /// <summary>Add the pointer to an element to a dynamic array.</summary>
    /// <param name="tab_ptr">Pointer to the array to grow</param>
    /// <param name="nb_ptr">Pointer to the number of elements in the array</param>
    /// <param name="elem">Element to add</param>
    public static void av_dynarray_add(void* @tab_ptr, int* @nb_ptr, void* @elem) => vectors.av_dynarray_add(@tab_ptr, @nb_ptr, @elem);
    
    /// <summary>Add an element to a dynamic array.</summary>
    /// <returns>&gt;=0 on success, negative otherwise</returns>
    public static int av_dynarray_add_nofree(void* @tab_ptr, int* @nb_ptr, void* @elem) => vectors.av_dynarray_add_nofree(@tab_ptr, @nb_ptr, @elem);
    
    /// <summary>Add an element of size `elem_size` to a dynamic array.</summary>
    /// <param name="tab_ptr">Pointer to the array to grow</param>
    /// <param name="nb_ptr">Pointer to the number of elements in the array</param>
    /// <param name="elem_size">Size in bytes of an element in the array</param>
    /// <param name="elem_data">Pointer to the data of the element to add. If `NULL`, the space of the newly added element is allocated but left uninitialized.</param>
    /// <returns>Pointer to the data of the element to copy in the newly allocated space</returns>
    public static void* av_dynarray2_add(void** @tab_ptr, int* @nb_ptr, ulong @elem_size, byte* @elem_data) => vectors.av_dynarray2_add(@tab_ptr, @nb_ptr, @elem_size, @elem_data);
    
    /// <summary>Allocate a buffer, reusing the given one if large enough.</summary>
    /// <param name="ptr">Pointer to pointer to an already allocated buffer. `*ptr` will be overwritten with pointer to new buffer on success or `NULL` on failure</param>
    /// <param name="size">Pointer to the size of buffer `*ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `*ptr`</param>
    public static void av_fast_malloc(void* @ptr, uint* @size, ulong @min_size) => vectors.av_fast_malloc(@ptr, @size, @min_size);
    
    /// <summary>Allocate and clear a buffer, reusing the given one if large enough.</summary>
    /// <param name="ptr">Pointer to pointer to an already allocated buffer. `*ptr` will be overwritten with pointer to new buffer on success or `NULL` on failure</param>
    /// <param name="size">Pointer to the size of buffer `*ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `*ptr`</param>
    public static void av_fast_mallocz(void* @ptr, uint* @size, ulong @min_size) => vectors.av_fast_mallocz(@ptr, @size, @min_size);
    
    /// <summary>Same behaviour av_fast_malloc but the buffer has additional AV_INPUT_BUFFER_PADDING_SIZE at the end which will always be 0.</summary>
    public static void av_fast_padded_malloc(void* @ptr, uint* @size, ulong @min_size) => vectors.av_fast_padded_malloc(@ptr, @size, @min_size);
    
    /// <summary>Same behaviour av_fast_padded_malloc except that buffer will always be 0-initialized after call.</summary>
    public static void av_fast_padded_mallocz(void* @ptr, uint* @size, ulong @min_size) => vectors.av_fast_padded_mallocz(@ptr, @size, @min_size);
    
    /// <summary>Reallocate the given buffer if it is not large enough, otherwise do nothing.</summary>
    /// <param name="ptr">Already allocated buffer, or `NULL`</param>
    /// <param name="size">Pointer to the size of buffer `ptr`. `*size` is updated to the new allocated size, in particular 0 in case of failure.</param>
    /// <param name="min_size">Desired minimal size of buffer `ptr`</param>
    /// <returns>`ptr` if the buffer is large enough, a pointer to newly reallocated buffer if the buffer was not large enough, or `NULL` in case of error</returns>
    public static void* av_fast_realloc(void* @ptr, uint* @size, ulong @min_size) => vectors.av_fast_realloc(@ptr, @size, @min_size);
    
    /// <summary>Read the file with name filename, and put its content in a newly allocated buffer or map it with mmap() when available. In case of success set *bufptr to the read or mmapped buffer, and *size to the size in bytes of the buffer in *bufptr. Unlike mmap this function succeeds with zero sized files, in this case *bufptr will be set to NULL and *size will be set to 0. The returned buffer must be released with av_file_unmap().</summary>
    /// <param name="log_offset">loglevel offset used for logging</param>
    /// <param name="log_ctx">context used for logging</param>
    /// <returns>a non negative number in case of success, a negative value corresponding to an AVERROR error code in case of failure</returns>
    public static int av_file_map(string @filename, byte** @bufptr, ulong* @size, int @log_offset, void* @log_ctx) => vectors.av_file_map(@filename, @bufptr, @size, @log_offset, @log_ctx);
    
    /// <summary>Unmap or free the buffer bufptr created by av_file_map().</summary>
    /// <param name="size">size in bytes of bufptr, must be the same as returned by av_file_map()</param>
    public static void av_file_unmap(byte* @bufptr, ulong @size) => vectors.av_file_unmap(@bufptr, @size);
    
    /// <summary>Check whether filename actually is a numbered sequence generator.</summary>
    /// <param name="filename">possible numbered sequence string</param>
    /// <returns>1 if a valid numbered sequence string, 0 otherwise</returns>
    public static int av_filename_number_test(string @filename) => vectors.av_filename_number_test(@filename);
    
    /// <summary>Iterate over all registered filters.</summary>
    /// <param name="opaque">a pointer where libavfilter will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered filter or NULL when the iteration is finished</returns>
    public static AVFilter* av_filter_iterate(void** @opaque) => vectors.av_filter_iterate(@opaque);
    
    /// <summary>Compute what kind of losses will occur when converting from one specific pixel format to another. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. These losses can involve loss of chroma, but also loss of resolution, loss of color depth, loss due to the color space conversion, loss of the alpha bits or loss due to color quantization. av_get_fix_fmt_loss() informs you about the various types of losses which will occur when converting from one pixel format to another.</summary>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <returns>Combination of flags informing you what kind of losses will occur (maximum loss for an invalid dst_pix_fmt).</returns>
    public static AVPixelFormat av_find_best_pix_fmt_of_2(AVPixelFormat @dst_pix_fmt1, AVPixelFormat @dst_pix_fmt2, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr) => vectors.av_find_best_pix_fmt_of_2(@dst_pix_fmt1, @dst_pix_fmt2, @src_pix_fmt, @has_alpha, @loss_ptr);
    
    /// <summary>Find the &quot;best&quot; stream in the file. The best stream is determined according to various heuristics as the most likely to be what the user expects. If the decoder parameter is non-NULL, av_find_best_stream will find the default decoder for the stream&apos;s codec; streams for which no decoder can be found are ignored.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="type">stream type: video, audio, subtitles, etc.</param>
    /// <param name="wanted_stream_nb">user-requested stream number, or -1 for automatic selection</param>
    /// <param name="related_stream">try to find a stream related (eg. in the same program) to this one, or -1 if none</param>
    /// <param name="decoder_ret">if non-NULL, returns the decoder for the selected stream</param>
    /// <param name="flags">flags; none are currently defined</param>
    /// <returns>the non-negative stream number in case of success, AVERROR_STREAM_NOT_FOUND if no stream with the requested type could be found, AVERROR_DECODER_NOT_FOUND if streams were found but no decoder</returns>
    public static int av_find_best_stream(AVFormatContext* @ic, AVMediaType @type, int @wanted_stream_nb, int @related_stream, AVCodec** @decoder_ret, int @flags) => vectors.av_find_best_stream(@ic, @type, @wanted_stream_nb, @related_stream, @decoder_ret, @flags);
    
    public static int av_find_default_stream_index(AVFormatContext* @s) => vectors.av_find_default_stream_index(@s);
    
    /// <summary>Find AVInputFormat based on the short name of the input format.</summary>
    public static AVInputFormat* av_find_input_format(string @short_name) => vectors.av_find_input_format(@short_name);
    
    /// <summary>Find the value in a list of rationals nearest a given reference rational.</summary>
    /// <param name="q">Reference rational</param>
    /// <param name="q_list">Array of rationals terminated by `{0, 0}`</param>
    /// <returns>Index of the nearest value found in the array</returns>
    public static int av_find_nearest_q_idx(AVRational @q, AVRational* @q_list) => vectors.av_find_nearest_q_idx(@q, @q_list);
    
    /// <summary>Find the programs which belong to a given stream.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="last">the last found program, the search will start after this program, or from the beginning if it is NULL</param>
    /// <param name="s">stream index</param>
    /// <returns>the next program which belongs to s, NULL if no program is found or the last program is not among the programs of ic.</returns>
    public static AVProgram* av_find_program_from_stream(AVFormatContext* @ic, AVProgram* @last, int @s) => vectors.av_find_program_from_stream(@ic, @last, @s);
    
    /// <summary>Returns the method used to set ctx-&gt;duration.</summary>
    /// <returns>AVFMT_DURATION_FROM_PTS, AVFMT_DURATION_FROM_STREAM, or AVFMT_DURATION_FROM_BITRATE.</returns>
    public static AVDurationEstimationMethod av_fmt_ctx_get_duration_estimation_method(AVFormatContext* @ctx) => vectors.av_fmt_ctx_get_duration_estimation_method(@ctx);
    
    /// <summary>Open a file using a UTF-8 filename. The API of this function matches POSIX fopen(), errors are returned through errno.</summary>
    [Obsolete("Avoid using it, as on Windows, the FILE* allocated by this function may be allocated with a different CRT than the caller who uses the FILE*. No replacement provided in public API.")]
    public static _iobuf* av_fopen_utf8(string @path, string @mode) => vectors.av_fopen_utf8(@path, @mode);
    
    /// <summary>Disables cpu detection and forces the specified flags. -1 is a special case that disables forcing of specific flags.</summary>
    public static void av_force_cpu_flags(int @flags) => vectors.av_force_cpu_flags(@flags);
    
    /// <summary>This function will cause global side data to be injected in the next packet of each stream as well as after any subsequent seek.</summary>
    public static void av_format_inject_global_side_data(AVFormatContext* @s) => vectors.av_format_inject_global_side_data(@s);
    
    /// <summary>Fill the provided buffer with a string containing a FourCC (four-character code) representation.</summary>
    /// <param name="buf">a buffer with size in bytes of at least AV_FOURCC_MAX_STRING_SIZE</param>
    /// <param name="fourcc">the fourcc to represent</param>
    /// <returns>the buffer in input</returns>
    public static byte* av_fourcc_make_string(byte* @buf, uint @fourcc) => vectors.av_fourcc_make_string(@buf, @fourcc);
    
    /// <summary>Allocate an AVFrame and set its fields to default values. The resulting struct must be freed using av_frame_free().</summary>
    /// <returns>An AVFrame filled with default values or NULL on failure.</returns>
    public static AVFrame* av_frame_alloc() => vectors.av_frame_alloc();
    
    /// <summary>Crop the given video AVFrame according to its crop_left/crop_top/crop_right/ crop_bottom fields. If cropping is successful, the function will adjust the data pointers and the width/height fields, and set the crop fields to 0.</summary>
    /// <param name="frame">the frame which should be cropped</param>
    /// <param name="flags">Some combination of AV_FRAME_CROP_* flags, or 0.</param>
    /// <returns>&gt;= 0 on success, a negative AVERROR on error. If the cropping fields were invalid, AVERROR(ERANGE) is returned, and nothing is changed.</returns>
    public static int av_frame_apply_cropping(AVFrame* @frame, int @flags) => vectors.av_frame_apply_cropping(@frame, @flags);
    
    /// <summary>Create a new frame that references the same data as src.</summary>
    /// <returns>newly created AVFrame on success, NULL on error.</returns>
    public static AVFrame* av_frame_clone(AVFrame* @src) => vectors.av_frame_clone(@src);
    
    /// <summary>Copy the frame data from src to dst.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR on error.</returns>
    public static int av_frame_copy(AVFrame* @dst, AVFrame* @src) => vectors.av_frame_copy(@dst, @src);
    
    /// <summary>Copy only &quot;metadata&quot; fields from src to dst.</summary>
    public static int av_frame_copy_props(AVFrame* @dst, AVFrame* @src) => vectors.av_frame_copy_props(@dst, @src);
    
    /// <summary>Free the frame and any dynamically allocated objects in it, e.g. extended_data. If the frame is reference counted, it will be unreferenced first.</summary>
    /// <param name="frame">frame to be freed. The pointer will be set to NULL.</param>
    public static void av_frame_free(AVFrame** @frame) => vectors.av_frame_free(@frame);
    
    /// <summary>Allocate new buffer(s) for audio or video data.</summary>
    /// <param name="frame">frame in which to store the new buffers.</param>
    /// <param name="align">Required buffer size alignment. If equal to 0, alignment will be chosen automatically for the current CPU. It is highly recommended to pass 0 here unless you know what you are doing.</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    public static int av_frame_get_buffer(AVFrame* @frame, int @align) => vectors.av_frame_get_buffer(@frame, @align);
    
    /// <summary>Get the buffer reference a given data plane is stored in.</summary>
    /// <param name="plane">index of the data plane of interest in frame-&gt;extended_data.</param>
    /// <returns>the buffer reference that contains the plane or NULL if the input frame is not valid.</returns>
    public static AVBufferRef* av_frame_get_plane_buffer(AVFrame* @frame, int @plane) => vectors.av_frame_get_plane_buffer(@frame, @plane);
    
    /// <summary>Returns a pointer to the side data of a given type on success, NULL if there is no side data with such type in this frame.</summary>
    /// <returns>a pointer to the side data of a given type on success, NULL if there is no side data with such type in this frame.</returns>
    public static AVFrameSideData* av_frame_get_side_data(AVFrame* @frame, AVFrameSideDataType @type) => vectors.av_frame_get_side_data(@frame, @type);
    
    /// <summary>Check if the frame data is writable.</summary>
    /// <returns>A positive value if the frame data is writable (which is true if and only if each of the underlying buffers has only one reference, namely the one stored in this frame). Return 0 otherwise.</returns>
    public static int av_frame_is_writable(AVFrame* @frame) => vectors.av_frame_is_writable(@frame);
    
    /// <summary>Ensure that the frame data is writable, avoiding data copy if possible.</summary>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    public static int av_frame_make_writable(AVFrame* @frame) => vectors.av_frame_make_writable(@frame);
    
    /// <summary>Move everything contained in src to dst and reset src.</summary>
    public static void av_frame_move_ref(AVFrame* @dst, AVFrame* @src) => vectors.av_frame_move_ref(@dst, @src);
    
    /// <summary>Add a new side data to a frame.</summary>
    /// <param name="frame">a frame to which the side data should be added</param>
    /// <param name="type">type of the added side data</param>
    /// <param name="size">size of the side data</param>
    /// <returns>newly added side data on success, NULL on error</returns>
    public static AVFrameSideData* av_frame_new_side_data(AVFrame* @frame, AVFrameSideDataType @type, ulong @size) => vectors.av_frame_new_side_data(@frame, @type, @size);
    
    /// <summary>Add a new side data to a frame from an existing AVBufferRef</summary>
    /// <param name="frame">a frame to which the side data should be added</param>
    /// <param name="type">the type of the added side data</param>
    /// <param name="buf">an AVBufferRef to add as side data. The ownership of the reference is transferred to the frame.</param>
    /// <returns>newly added side data on success, NULL on error. On failure the frame is unchanged and the AVBufferRef remains owned by the caller.</returns>
    public static AVFrameSideData* av_frame_new_side_data_from_buf(AVFrame* @frame, AVFrameSideDataType @type, AVBufferRef* @buf) => vectors.av_frame_new_side_data_from_buf(@frame, @type, @buf);
    
    /// <summary>Set up a new reference to the data described by the source frame.</summary>
    /// <returns>0 on success, a negative AVERROR on error</returns>
    public static int av_frame_ref(AVFrame* @dst, AVFrame* @src) => vectors.av_frame_ref(@dst, @src);
    
    /// <summary>Remove and free all side data instances of the given type.</summary>
    public static void av_frame_remove_side_data(AVFrame* @frame, AVFrameSideDataType @type) => vectors.av_frame_remove_side_data(@frame, @type);
    
    /// <summary>Returns a string identifying the side data type</summary>
    /// <returns>a string identifying the side data type</returns>
    public static string av_frame_side_data_name(AVFrameSideDataType @type) => vectors.av_frame_side_data_name(@type);
    
    /// <summary>Unreference all the buffers referenced by frame and reset the frame fields.</summary>
    public static void av_frame_unref(AVFrame* @frame) => vectors.av_frame_unref(@frame);
    
    /// <summary>Free a memory block which has been allocated with a function of av_malloc() or av_realloc() family.</summary>
    /// <param name="ptr">Pointer to the memory block which should be freed.</param>
    public static void av_free(void* @ptr) => vectors.av_free(@ptr);
    
    /// <summary>Free a memory block which has been allocated with a function of av_malloc() or av_realloc() family, and set the pointer pointing to it to `NULL`.</summary>
    /// <param name="ptr">Pointer to the pointer to the memory block which should be freed</param>
    public static void av_freep(void* @ptr) => vectors.av_freep(@ptr);
    
    /// <summary>Compute the greatest common divisor of two integer operands.</summary>
    /// <returns>GCD of a and b up to sign; if a &gt;= 0 and b &gt;= 0, return value is &gt;= 0; if a == 0 and b == 0, returns 0.</returns>
    public static long av_gcd(long @a, long @b) => vectors.av_gcd(@a, @b);
    
    /// <summary>Return the best rational so that a and b are multiple of it. If the resulting denominator is larger than max_den, return def.</summary>
    public static AVRational av_gcd_q(AVRational @a, AVRational @b, int @max_den, AVRational @def) => vectors.av_gcd_q(@a, @b, @max_den, @def);
    
    /// <summary>Return the planar&lt;-&gt;packed alternative form of the given sample format, or AV_SAMPLE_FMT_NONE on error. If the passed sample_fmt is already in the requested planar/packed format, the format returned is the same as the input.</summary>
    public static AVSampleFormat av_get_alt_sample_fmt(AVSampleFormat @sample_fmt, int @planar) => vectors.av_get_alt_sample_fmt(@sample_fmt, @planar);
    
    /// <summary>Return audio frame duration.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame_bytes">size of the frame, or 0 if unknown</param>
    /// <returns>frame duration, in samples, if known. 0 if not able to determine.</returns>
    public static int av_get_audio_frame_duration(AVCodecContext* @avctx, int @frame_bytes) => vectors.av_get_audio_frame_duration(@avctx, @frame_bytes);
    
    /// <summary>This function is the same as av_get_audio_frame_duration(), except it works with AVCodecParameters instead of an AVCodecContext.</summary>
    public static int av_get_audio_frame_duration2(AVCodecParameters* @par, int @frame_bytes) => vectors.av_get_audio_frame_duration2(@par, @frame_bytes);
    
    /// <summary>Return the number of bits per pixel used by the pixel format described by pixdesc. Note that this is not the same as the number of bits per sample.</summary>
    public static int av_get_bits_per_pixel(AVPixFmtDescriptor* @pixdesc) => vectors.av_get_bits_per_pixel(@pixdesc);
    
    /// <summary>Return codec bits per sample.</summary>
    /// <param name="codec_id">the codec</param>
    /// <returns>Number of bits per sample or zero if unknown for the given codec.</returns>
    public static int av_get_bits_per_sample(AVCodecID @codec_id) => vectors.av_get_bits_per_sample(@codec_id);
    
    /// <summary>Return number of bytes per sample.</summary>
    /// <param name="sample_fmt">the sample format</param>
    /// <returns>number of bytes per sample or zero if unknown for the given sample format</returns>
    public static int av_get_bytes_per_sample(AVSampleFormat @sample_fmt) => vectors.av_get_bytes_per_sample(@sample_fmt);
    
    /// <summary>Get the description of a given channel.</summary>
    /// <param name="channel">a channel layout with a single channel</param>
    /// <returns>channel description on success, NULL on error</returns>
    [Obsolete("use av_channel_description()")]
    public static string av_get_channel_description(ulong @channel) => vectors.av_get_channel_description(@channel);
    
    /// <summary>Return a channel layout id that matches name, or 0 if no match is found.</summary>
    [Obsolete("use av_channel_layout_from_string()")]
    public static ulong av_get_channel_layout(string @name) => vectors.av_get_channel_layout(@name);
    
    /// <summary>Get the index of a channel in channel_layout.</summary>
    /// <param name="channel">a channel layout describing exactly one channel which must be present in channel_layout.</param>
    /// <returns>index of channel in channel_layout on success, a negative AVERROR on error.</returns>
    [Obsolete("use av_channel_layout_index_from_channel()")]
    public static int av_get_channel_layout_channel_index(ulong @channel_layout, ulong @channel) => vectors.av_get_channel_layout_channel_index(@channel_layout, @channel);
    
    /// <summary>Return the number of channels in the channel layout.</summary>
    [Obsolete("use AVChannelLayout.nb_channels")]
    public static int av_get_channel_layout_nb_channels(ulong @channel_layout) => vectors.av_get_channel_layout_nb_channels(@channel_layout);
    
    /// <summary>Return a description of a channel layout. If nb_channels is &lt;= 0, it is guessed from the channel_layout.</summary>
    /// <param name="buf">put here the string containing the channel layout</param>
    /// <param name="buf_size">size in bytes of the buffer</param>
    [Obsolete("use av_channel_layout_describe()")]
    public static void av_get_channel_layout_string(byte* @buf, int @buf_size, int @nb_channels, ulong @channel_layout) => vectors.av_get_channel_layout_string(@buf, @buf_size, @nb_channels, @channel_layout);
    
    /// <summary>Get the name of a given channel.</summary>
    /// <returns>channel name on success, NULL on error.</returns>
    [Obsolete("use av_channel_name()")]
    public static string av_get_channel_name(ulong @channel) => vectors.av_get_channel_name(@channel);
    
    /// <summary>Get the name of a colorspace.</summary>
    /// <returns>a static string identifying the colorspace; can be NULL.</returns>
    [Obsolete("use av_color_space_name()")]
    public static string av_get_colorspace_name(AVColorSpace @val) => vectors.av_get_colorspace_name(@val);
    
    /// <summary>Return the flags which specify extensions supported by the CPU. The returned value is affected by av_force_cpu_flags() if that was used before. So av_get_cpu_flags() can easily be used in an application to detect the enabled cpu flags.</summary>
    public static int av_get_cpu_flags() => vectors.av_get_cpu_flags();
    
    /// <summary>Return default channel layout for a given number of channels.</summary>
    [Obsolete("use av_channel_layout_default()")]
    public static long av_get_default_channel_layout(int @nb_channels) => vectors.av_get_default_channel_layout(@nb_channels);
    
    /// <summary>Return codec bits per sample. Only return non-zero if the bits per sample is exactly correct, not an approximation.</summary>
    /// <param name="codec_id">the codec</param>
    /// <returns>Number of bits per sample or zero if unknown for the given codec.</returns>
    public static int av_get_exact_bits_per_sample(AVCodecID @codec_id) => vectors.av_get_exact_bits_per_sample(@codec_id);
    
    /// <summary>Return a channel layout and the number of channels based on the specified name.</summary>
    /// <param name="name">channel layout specification string</param>
    /// <param name="channel_layout">parsed channel layout (0 if unknown)</param>
    /// <param name="nb_channels">number of channels</param>
    /// <returns>0 on success, AVERROR(EINVAL) if the parsing fails.</returns>
    [Obsolete("use av_channel_layout_from_string()")]
    public static int av_get_extended_channel_layout(string @name, ulong* @channel_layout, int* @nb_channels) => vectors.av_get_extended_channel_layout(@name, @channel_layout, @nb_channels);
    
    public static int av_get_frame_filename(byte* @buf, int @buf_size, string @path, int @number) => vectors.av_get_frame_filename(@buf, @buf_size, @path, @number);
    
    /// <summary>Return in &apos;buf&apos; the path with &apos;%d&apos; replaced by a number.</summary>
    /// <param name="buf">destination buffer</param>
    /// <param name="buf_size">destination buffer size</param>
    /// <param name="path">numbered sequence string</param>
    /// <param name="number">frame number</param>
    /// <param name="flags">AV_FRAME_FILENAME_FLAGS_*</param>
    /// <returns>0 if OK, -1 on format error</returns>
    public static int av_get_frame_filename2(byte* @buf, int @buf_size, string @path, int @number, int @flags) => vectors.av_get_frame_filename2(@buf, @buf_size, @path, @number, @flags);
    
    /// <summary>Return a string describing the media_type enum, NULL if media_type is unknown.</summary>
    public static string av_get_media_type_string(AVMediaType @media_type) => vectors.av_get_media_type_string(@media_type);
    
    /// <summary>Get timing information for the data currently output. The exact meaning of &quot;currently output&quot; depends on the format. It is mostly relevant for devices that have an internal buffer and/or work in real time.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream">stream in the media file</param>
    /// <param name="dts">DTS of the last packet output for the stream, in stream time_base units</param>
    /// <param name="wall">absolute time when that packet whas output, in microsecond</param>
    /// <returns>0 if OK, AVERROR(ENOSYS) if the format does not support it Note: some formats or devices may not allow to measure dts and wall atomically.</returns>
    public static int av_get_output_timestamp(AVFormatContext* @s, int @stream, long* @dts, long* @wall) => vectors.av_get_output_timestamp(@s, @stream, @dts, @wall);
    
    /// <summary>Get the packed alternative form of the given sample format.</summary>
    /// <returns>the packed alternative form of the given sample format or AV_SAMPLE_FMT_NONE on error.</returns>
    public static AVSampleFormat av_get_packed_sample_fmt(AVSampleFormat @sample_fmt) => vectors.av_get_packed_sample_fmt(@sample_fmt);
    
    /// <summary>Allocate and read the payload of a packet and initialize its fields with default values.</summary>
    /// <param name="s">associated IO context</param>
    /// <param name="pkt">packet</param>
    /// <param name="size">desired payload size</param>
    /// <returns>&gt;0 (read size) if OK, AVERROR_xxx otherwise</returns>
    public static int av_get_packet(AVIOContext* @s, AVPacket* @pkt, int @size) => vectors.av_get_packet(@s, @pkt, @size);
    
    /// <summary>Return the number of bits per pixel for the pixel format described by pixdesc, including any padding or unused bits.</summary>
    public static int av_get_padded_bits_per_pixel(AVPixFmtDescriptor* @pixdesc) => vectors.av_get_padded_bits_per_pixel(@pixdesc);
    
    /// <summary>Return the PCM codec associated with a sample format.</summary>
    /// <param name="be">endianness, 0 for little, 1 for big, -1 (or anything else) for native</param>
    /// <returns>AV_CODEC_ID_PCM_* or AV_CODEC_ID_NONE</returns>
    public static AVCodecID av_get_pcm_codec(AVSampleFormat @fmt, int @be) => vectors.av_get_pcm_codec(@fmt, @be);
    
    /// <summary>Return a single letter to describe the given picture type pict_type.</summary>
    /// <param name="pict_type">the picture type</param>
    /// <returns>a single character representing the picture type, &apos;?&apos; if pict_type is unknown</returns>
    public static byte av_get_picture_type_char(AVPictureType @pict_type) => vectors.av_get_picture_type_char(@pict_type);
    
    /// <summary>Return the pixel format corresponding to name.</summary>
    public static AVPixelFormat av_get_pix_fmt(string @name) => vectors.av_get_pix_fmt(@name);
    
    /// <summary>Compute what kind of losses will occur when converting from one specific pixel format to another. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. These losses can involve loss of chroma, but also loss of resolution, loss of color depth, loss due to the color space conversion, loss of the alpha bits or loss due to color quantization. av_get_fix_fmt_loss() informs you about the various types of losses which will occur when converting from one pixel format to another.</summary>
    /// <param name="dst_pix_fmt">destination pixel format</param>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <returns>Combination of flags informing you what kind of losses will occur (maximum loss for an invalid dst_pix_fmt).</returns>
    public static int av_get_pix_fmt_loss(AVPixelFormat @dst_pix_fmt, AVPixelFormat @src_pix_fmt, int @has_alpha) => vectors.av_get_pix_fmt_loss(@dst_pix_fmt, @src_pix_fmt, @has_alpha);
    
    /// <summary>Return the short name for a pixel format, NULL in case pix_fmt is unknown.</summary>
    public static string av_get_pix_fmt_name(AVPixelFormat @pix_fmt) => vectors.av_get_pix_fmt_name(@pix_fmt);
    
    /// <summary>Print in buf the string corresponding to the pixel format with number pix_fmt, or a header if pix_fmt is negative.</summary>
    /// <param name="buf">the buffer where to write the string</param>
    /// <param name="buf_size">the size of buf</param>
    /// <param name="pix_fmt">the number of the pixel format to print the corresponding info string, or a negative value to print the corresponding header.</param>
    public static byte* av_get_pix_fmt_string(byte* @buf, int @buf_size, AVPixelFormat @pix_fmt) => vectors.av_get_pix_fmt_string(@buf, @buf_size, @pix_fmt);
    
    /// <summary>Get the planar alternative form of the given sample format.</summary>
    /// <returns>the planar alternative form of the given sample format or AV_SAMPLE_FMT_NONE on error.</returns>
    public static AVSampleFormat av_get_planar_sample_fmt(AVSampleFormat @sample_fmt) => vectors.av_get_planar_sample_fmt(@sample_fmt);
    
    /// <summary>Return a name for the specified profile, if available.</summary>
    /// <param name="codec">the codec that is searched for the given profile</param>
    /// <param name="profile">the profile value for which a name is requested</param>
    /// <returns>A name for the profile if found, NULL otherwise.</returns>
    public static string av_get_profile_name(AVCodec* @codec, int @profile) => vectors.av_get_profile_name(@codec, @profile);
    
    /// <summary>Return a sample format corresponding to name, or AV_SAMPLE_FMT_NONE on error.</summary>
    public static AVSampleFormat av_get_sample_fmt(string @name) => vectors.av_get_sample_fmt(@name);
    
    /// <summary>Return the name of sample_fmt, or NULL if sample_fmt is not recognized.</summary>
    public static string av_get_sample_fmt_name(AVSampleFormat @sample_fmt) => vectors.av_get_sample_fmt_name(@sample_fmt);
    
    /// <summary>Generate a string corresponding to the sample format with sample_fmt, or a header if sample_fmt is negative.</summary>
    /// <param name="buf">the buffer where to write the string</param>
    /// <param name="buf_size">the size of buf</param>
    /// <param name="sample_fmt">the number of the sample format to print the corresponding info string, or a negative value to print the corresponding header.</param>
    /// <returns>the pointer to the filled buffer or NULL if sample_fmt is unknown or in case of other errors</returns>
    public static byte* av_get_sample_fmt_string(byte* @buf, int @buf_size, AVSampleFormat @sample_fmt) => vectors.av_get_sample_fmt_string(@buf, @buf_size, @sample_fmt);
    
    /// <summary>Get the value and name of a standard channel layout.</summary>
    /// <param name="index">index in an internal list, starting at 0</param>
    /// <param name="layout">channel layout mask</param>
    /// <param name="name">name of the layout</param>
    /// <returns>0  if the layout exists,  &lt; 0 if index is beyond the limits</returns>
    [Obsolete("use av_channel_layout_standard()")]
    public static int av_get_standard_channel_layout(uint @index, ulong* @layout, byte** @name) => vectors.av_get_standard_channel_layout(@index, @layout, @name);
    
    /// <summary>Return the fractional representation of the internal time base.</summary>
    public static AVRational av_get_time_base_q() => vectors.av_get_time_base_q();
    
    /// <summary>Get the current time in microseconds.</summary>
    public static long av_gettime() => vectors.av_gettime();
    
    /// <summary>Get the current time in microseconds since some unspecified starting point. On platforms that support it, the time comes from a monotonic clock This property makes this time source ideal for measuring relative time. The returned values may not be monotonic on platforms where a monotonic clock is not available.</summary>
    public static long av_gettime_relative() => vectors.av_gettime_relative();
    
    /// <summary>Indicates with a boolean result if the av_gettime_relative() time source is monotonic.</summary>
    public static int av_gettime_relative_is_monotonic() => vectors.av_gettime_relative_is_monotonic();
    
    /// <summary>Increase packet size, correctly zeroing padding</summary>
    /// <param name="pkt">packet</param>
    /// <param name="grow_by">number of bytes by which to increase the size of the packet</param>
    public static int av_grow_packet(AVPacket* @pkt, int @grow_by) => vectors.av_grow_packet(@pkt, @grow_by);
    
    /// <summary>Guess the codec ID based upon muxer and filename.</summary>
    public static AVCodecID av_guess_codec(AVOutputFormat* @fmt, string @short_name, string @filename, string @mime_type, AVMediaType @type) => vectors.av_guess_codec(@fmt, @short_name, @filename, @mime_type, @type);
    
    /// <summary>Return the output format in the list of registered output formats which best matches the provided parameters, or return NULL if there is no match.</summary>
    /// <param name="short_name">if non-NULL checks if short_name matches with the names of the registered formats</param>
    /// <param name="filename">if non-NULL checks if filename terminates with the extensions of the registered formats</param>
    /// <param name="mime_type">if non-NULL checks if mime_type matches with the MIME type of the registered formats</param>
    public static AVOutputFormat* av_guess_format(string @short_name, string @filename, string @mime_type) => vectors.av_guess_format(@short_name, @filename, @mime_type);
    
    /// <summary>Guess the frame rate, based on both the container and codec information.</summary>
    /// <param name="ctx">the format context which the stream is part of</param>
    /// <param name="stream">the stream which the frame is part of</param>
    /// <param name="frame">the frame for which the frame rate should be determined, may be NULL</param>
    /// <returns>the guessed (valid) frame rate, 0/1 if no idea</returns>
    public static AVRational av_guess_frame_rate(AVFormatContext* @ctx, AVStream* @stream, AVFrame* @frame) => vectors.av_guess_frame_rate(@ctx, @stream, @frame);
    
    /// <summary>Guess the sample aspect ratio of a frame, based on both the stream and the frame aspect ratio.</summary>
    /// <param name="format">the format context which the stream is part of</param>
    /// <param name="stream">the stream which the frame is part of</param>
    /// <param name="frame">the frame with the aspect ratio to be determined</param>
    /// <returns>the guessed (valid) sample_aspect_ratio, 0/1 if no idea</returns>
    public static AVRational av_guess_sample_aspect_ratio(AVFormatContext* @format, AVStream* @stream, AVFrame* @frame) => vectors.av_guess_sample_aspect_ratio(@format, @stream, @frame);
    
    /// <summary>Send a nice hexadecimal dump of a buffer to the specified file stream.</summary>
    /// <param name="f">The file stream pointer where the dump should be sent to.</param>
    /// <param name="buf">buffer</param>
    /// <param name="size">buffer size</param>
    public static void av_hex_dump(_iobuf* @f, byte* @buf, int @size) => vectors.av_hex_dump(@f, @buf, @size);
    
    /// <summary>Send a nice hexadecimal dump of a buffer to the log.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message, lower values signifying higher importance.</param>
    /// <param name="buf">buffer</param>
    /// <param name="size">buffer size</param>
    public static void av_hex_dump_log(void* @avcl, int @level, byte* @buf, int @size) => vectors.av_hex_dump_log(@avcl, @level, @buf, @size);
    
    /// <summary>Allocate an AVHWDeviceContext for a given hardware type.</summary>
    /// <param name="type">the type of the hardware device to allocate.</param>
    /// <returns>a reference to the newly created AVHWDeviceContext on success or NULL on failure.</returns>
    public static AVBufferRef* av_hwdevice_ctx_alloc(AVHWDeviceType @type) => vectors.av_hwdevice_ctx_alloc(@type);
    
    /// <summary>Open a device of the specified type and create an AVHWDeviceContext for it.</summary>
    /// <param name="device_ctx">On success, a reference to the newly-created device context will be written here. The reference is owned by the caller and must be released with av_buffer_unref() when no longer needed. On failure, NULL will be written to this pointer.</param>
    /// <param name="type">The type of the device to create.</param>
    /// <param name="device">A type-specific string identifying the device to open.</param>
    /// <param name="opts">A dictionary of additional (type-specific) options to use in opening the device. The dictionary remains owned by the caller.</param>
    /// <param name="flags">currently unused</param>
    /// <returns>0 on success, a negative AVERROR code on failure.</returns>
    public static int av_hwdevice_ctx_create(AVBufferRef** @device_ctx, AVHWDeviceType @type, string @device, AVDictionary* @opts, int @flags) => vectors.av_hwdevice_ctx_create(@device_ctx, @type, @device, @opts, @flags);
    
    /// <summary>Create a new device of the specified type from an existing device.</summary>
    /// <param name="dst_ctx">On success, a reference to the newly-created AVHWDeviceContext.</param>
    /// <param name="type">The type of the new device to create.</param>
    /// <param name="src_ctx">A reference to an existing AVHWDeviceContext which will be used to create the new device.</param>
    /// <param name="flags">Currently unused; should be set to zero.</param>
    /// <returns>Zero on success, a negative AVERROR code on failure.</returns>
    public static int av_hwdevice_ctx_create_derived(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, int @flags) => vectors.av_hwdevice_ctx_create_derived(@dst_ctx, @type, @src_ctx, @flags);
    
    /// <summary>Create a new device of the specified type from an existing device.</summary>
    /// <param name="dst_ctx">On success, a reference to the newly-created AVHWDeviceContext.</param>
    /// <param name="type">The type of the new device to create.</param>
    /// <param name="src_ctx">A reference to an existing AVHWDeviceContext which will be used to create the new device.</param>
    /// <param name="options">Options for the new device to create, same format as in av_hwdevice_ctx_create.</param>
    /// <param name="flags">Currently unused; should be set to zero.</param>
    /// <returns>Zero on success, a negative AVERROR code on failure.</returns>
    public static int av_hwdevice_ctx_create_derived_opts(AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, AVDictionary* @options, int @flags) => vectors.av_hwdevice_ctx_create_derived_opts(@dst_ctx, @type, @src_ctx, @options, @flags);
    
    /// <summary>Finalize the device context before use. This function must be called after the context is filled with all the required information and before it is used in any way.</summary>
    /// <param name="ref">a reference to the AVHWDeviceContext</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int av_hwdevice_ctx_init(AVBufferRef* @ref) => vectors.av_hwdevice_ctx_init(@ref);
    
    /// <summary>Look up an AVHWDeviceType by name.</summary>
    /// <param name="name">String name of the device type (case-insensitive).</param>
    /// <returns>The type from enum AVHWDeviceType, or AV_HWDEVICE_TYPE_NONE if not found.</returns>
    public static AVHWDeviceType av_hwdevice_find_type_by_name(string @name) => vectors.av_hwdevice_find_type_by_name(@name);
    
    /// <summary>Get the constraints on HW frames given a device and the HW-specific configuration to be used with that device. If no HW-specific configuration is provided, returns the maximum possible capabilities of the device.</summary>
    /// <param name="ref">a reference to the associated AVHWDeviceContext.</param>
    /// <param name="hwconfig">a filled HW-specific configuration structure, or NULL to return the maximum possible capabilities of the device.</param>
    /// <returns>AVHWFramesConstraints structure describing the constraints on the device, or NULL if not available.</returns>
    public static AVHWFramesConstraints* av_hwdevice_get_hwframe_constraints(AVBufferRef* @ref, void* @hwconfig) => vectors.av_hwdevice_get_hwframe_constraints(@ref, @hwconfig);
    
    /// <summary>Get the string name of an AVHWDeviceType.</summary>
    /// <param name="type">Type from enum AVHWDeviceType.</param>
    /// <returns>Pointer to a static string containing the name, or NULL if the type is not valid.</returns>
    public static string av_hwdevice_get_type_name(AVHWDeviceType @type) => vectors.av_hwdevice_get_type_name(@type);
    
    /// <summary>Allocate a HW-specific configuration structure for a given HW device. After use, the user must free all members as required by the specific hardware structure being used, then free the structure itself with av_free().</summary>
    /// <param name="device_ctx">a reference to the associated AVHWDeviceContext.</param>
    /// <returns>The newly created HW-specific configuration structure on success or NULL on failure.</returns>
    public static void* av_hwdevice_hwconfig_alloc(AVBufferRef* @device_ctx) => vectors.av_hwdevice_hwconfig_alloc(@device_ctx);
    
    /// <summary>Iterate over supported device types.</summary>
    /// <returns>The next usable device type from enum AVHWDeviceType, or AV_HWDEVICE_TYPE_NONE if there are no more.</returns>
    public static AVHWDeviceType av_hwdevice_iterate_types(AVHWDeviceType @prev) => vectors.av_hwdevice_iterate_types(@prev);
    
    /// <summary>Free an AVHWFrameConstraints structure.</summary>
    /// <param name="constraints">The (filled or unfilled) AVHWFrameConstraints structure.</param>
    public static void av_hwframe_constraints_free(AVHWFramesConstraints** @constraints) => vectors.av_hwframe_constraints_free(@constraints);
    
    /// <summary>Allocate an AVHWFramesContext tied to a given device context.</summary>
    /// <param name="device_ctx">a reference to a AVHWDeviceContext. This function will make a new reference for internal use, the one passed to the function remains owned by the caller.</param>
    /// <returns>a reference to the newly created AVHWFramesContext on success or NULL on failure.</returns>
    public static AVBufferRef* av_hwframe_ctx_alloc(AVBufferRef* @device_ctx) => vectors.av_hwframe_ctx_alloc(@device_ctx);
    
    /// <summary>Create and initialise an AVHWFramesContext as a mapping of another existing AVHWFramesContext on a different device.</summary>
    /// <param name="derived_frame_ctx">On success, a reference to the newly created AVHWFramesContext.</param>
    /// <param name="derived_device_ctx">A reference to the device to create the new AVHWFramesContext on.</param>
    /// <param name="source_frame_ctx">A reference to an existing AVHWFramesContext which will be mapped to the derived context.</param>
    /// <param name="flags">Some combination of AV_HWFRAME_MAP_* flags, defining the mapping parameters to apply to frames which are allocated in the derived device.</param>
    /// <returns>Zero on success, negative AVERROR code on failure.</returns>
    public static int av_hwframe_ctx_create_derived(AVBufferRef** @derived_frame_ctx, AVPixelFormat @format, AVBufferRef* @derived_device_ctx, AVBufferRef* @source_frame_ctx, int @flags) => vectors.av_hwframe_ctx_create_derived(@derived_frame_ctx, @format, @derived_device_ctx, @source_frame_ctx, @flags);
    
    /// <summary>Finalize the context before use. This function must be called after the context is filled with all the required information and before it is attached to any frames.</summary>
    /// <param name="ref">a reference to the AVHWFramesContext</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int av_hwframe_ctx_init(AVBufferRef* @ref) => vectors.av_hwframe_ctx_init(@ref);
    
    /// <summary>Allocate a new frame attached to the given AVHWFramesContext.</summary>
    /// <param name="hwframe_ctx">a reference to an AVHWFramesContext</param>
    /// <param name="frame">an empty (freshly allocated or unreffed) frame to be filled with newly allocated buffers.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int av_hwframe_get_buffer(AVBufferRef* @hwframe_ctx, AVFrame* @frame, int @flags) => vectors.av_hwframe_get_buffer(@hwframe_ctx, @frame, @flags);
    
    /// <summary>Map a hardware frame.</summary>
    /// <param name="dst">Destination frame, to contain the mapping.</param>
    /// <param name="src">Source frame, to be mapped.</param>
    /// <param name="flags">Some combination of AV_HWFRAME_MAP_* flags.</param>
    /// <returns>Zero on success, negative AVERROR code on failure.</returns>
    public static int av_hwframe_map(AVFrame* @dst, AVFrame* @src, int @flags) => vectors.av_hwframe_map(@dst, @src, @flags);
    
    /// <summary>Copy data to or from a hw surface. At least one of dst/src must have an AVHWFramesContext attached.</summary>
    /// <param name="dst">the destination frame. dst is not touched on failure.</param>
    /// <param name="src">the source frame.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR error code on failure.</returns>
    public static int av_hwframe_transfer_data(AVFrame* @dst, AVFrame* @src, int @flags) => vectors.av_hwframe_transfer_data(@dst, @src, @flags);
    
    /// <summary>Get a list of possible source or target formats usable in av_hwframe_transfer_data().</summary>
    /// <param name="hwframe_ctx">the frame context to obtain the information for</param>
    /// <param name="dir">the direction of the transfer</param>
    /// <param name="formats">the pointer to the output format list will be written here. The list is terminated with AV_PIX_FMT_NONE and must be freed by the caller when no longer needed using av_free(). If this function returns successfully, the format list will have at least one item (not counting the terminator). On failure, the contents of this pointer are unspecified.</param>
    /// <param name="flags">currently unused, should be set to zero</param>
    /// <returns>0 on success, a negative AVERROR code on failure.</returns>
    public static int av_hwframe_transfer_get_formats(AVBufferRef* @hwframe_ctx, AVHWFrameTransferDirection @dir, AVPixelFormat** @formats, int @flags) => vectors.av_hwframe_transfer_get_formats(@hwframe_ctx, @dir, @formats, @flags);
    
    /// <summary>Allocate an image with size w and h and pixel format pix_fmt, and fill pointers and linesizes accordingly. The allocated image buffer has to be freed by using av_freep(&amp;pointers[0]).</summary>
    /// <param name="align">the value to use for buffer size alignment</param>
    /// <returns>the size in bytes required for the image buffer, a negative error code in case of failure</returns>
    public static int av_image_alloc(ref byte_ptrArray4 @pointers, ref int_array4 @linesizes, int @w, int @h, AVPixelFormat @pix_fmt, int @align) => vectors.av_image_alloc(ref @pointers, ref @linesizes, @w, @h, @pix_fmt, @align);
    
    /// <summary>Check if the given sample aspect ratio of an image is valid.</summary>
    /// <param name="w">width of the image</param>
    /// <param name="h">height of the image</param>
    /// <param name="sar">sample aspect ratio of the image</param>
    /// <returns>0 if valid, a negative AVERROR code otherwise</returns>
    public static int av_image_check_sar(uint @w, uint @h, AVRational @sar) => vectors.av_image_check_sar(@w, @h, @sar);
    
    /// <summary>Check if the given dimension of an image is valid, meaning that all bytes of the image can be addressed with a signed int.</summary>
    /// <param name="w">the width of the picture</param>
    /// <param name="h">the height of the picture</param>
    /// <param name="log_offset">the offset to sum to the log level for logging with log_ctx</param>
    /// <param name="log_ctx">the parent logging context, it may be NULL</param>
    /// <returns>&gt;= 0 if valid, a negative error code otherwise</returns>
    public static int av_image_check_size(uint @w, uint @h, int @log_offset, void* @log_ctx) => vectors.av_image_check_size(@w, @h, @log_offset, @log_ctx);
    
    /// <summary>Check if the given dimension of an image is valid, meaning that all bytes of a plane of an image with the specified pix_fmt can be addressed with a signed int.</summary>
    /// <param name="w">the width of the picture</param>
    /// <param name="h">the height of the picture</param>
    /// <param name="max_pixels">the maximum number of pixels the user wants to accept</param>
    /// <param name="pix_fmt">the pixel format, can be AV_PIX_FMT_NONE if unknown.</param>
    /// <param name="log_offset">the offset to sum to the log level for logging with log_ctx</param>
    /// <param name="log_ctx">the parent logging context, it may be NULL</param>
    /// <returns>&gt;= 0 if valid, a negative error code otherwise</returns>
    public static int av_image_check_size2(uint @w, uint @h, long @max_pixels, AVPixelFormat @pix_fmt, int @log_offset, void* @log_ctx) => vectors.av_image_check_size2(@w, @h, @max_pixels, @pix_fmt, @log_offset, @log_ctx);
    
    /// <summary>Copy image in src_data to dst_data.</summary>
    /// <param name="dst_linesizes">linesizes for the image in dst_data</param>
    /// <param name="src_linesizes">linesizes for the image in src_data</param>
    public static void av_image_copy(ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in int_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height) => vectors.av_image_copy(ref @dst_data, ref @dst_linesizes, @src_data, @src_linesizes, @pix_fmt, @width, @height);
    
    /// <summary>Copy image plane from src to dst. That is, copy &quot;height&quot; number of lines of &quot;bytewidth&quot; bytes each. The first byte of each successive line is separated by *_linesize bytes.</summary>
    /// <param name="dst_linesize">linesize for the image plane in dst</param>
    /// <param name="src_linesize">linesize for the image plane in src</param>
    public static void av_image_copy_plane(byte* @dst, int @dst_linesize, byte* @src, int @src_linesize, int @bytewidth, int @height) => vectors.av_image_copy_plane(@dst, @dst_linesize, @src, @src_linesize, @bytewidth, @height);
    
    /// <summary>Copy image data located in uncacheable (e.g. GPU mapped) memory. Where available, this function will use special functionality for reading from such memory, which may result in greatly improved performance compared to plain av_image_copy_plane().</summary>
    public static void av_image_copy_plane_uc_from(byte* @dst, long @dst_linesize, byte* @src, long @src_linesize, long @bytewidth, int @height) => vectors.av_image_copy_plane_uc_from(@dst, @dst_linesize, @src, @src_linesize, @bytewidth, @height);
    
    /// <summary>Copy image data from an image into a buffer.</summary>
    /// <param name="dst">a buffer into which picture data will be copied</param>
    /// <param name="dst_size">the size in bytes of dst</param>
    /// <param name="src_data">pointers containing the source image data</param>
    /// <param name="src_linesize">linesizes for the image in src_data</param>
    /// <param name="pix_fmt">the pixel format of the source image</param>
    /// <param name="width">the width of the source image in pixels</param>
    /// <param name="height">the height of the source image in pixels</param>
    /// <param name="align">the assumed linesize alignment for dst</param>
    /// <returns>the number of bytes written to dst, or a negative value (error code) on error</returns>
    public static int av_image_copy_to_buffer(byte* @dst, int @dst_size, in byte_ptrArray4 @src_data, in int_array4 @src_linesize, AVPixelFormat @pix_fmt, int @width, int @height, int @align) => vectors.av_image_copy_to_buffer(@dst, @dst_size, @src_data, @src_linesize, @pix_fmt, @width, @height, @align);
    
    /// <summary>Copy image data located in uncacheable (e.g. GPU mapped) memory. Where available, this function will use special functionality for reading from such memory, which may result in greatly improved performance compared to plain av_image_copy().</summary>
    public static void av_image_copy_uc_from(ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in long_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height) => vectors.av_image_copy_uc_from(ref @dst_data, @dst_linesizes, @src_data, @src_linesizes, @pix_fmt, @width, @height);
    
    /// <summary>Setup the data pointers and linesizes based on the specified image parameters and the provided array.</summary>
    /// <param name="dst_data">data pointers to be filled in</param>
    /// <param name="dst_linesize">linesizes for the image in dst_data to be filled in</param>
    /// <param name="src">buffer which will contain or contains the actual image data, can be NULL</param>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="align">the value used in src for linesize alignment</param>
    /// <returns>the size in bytes required for src, a negative error code in case of failure</returns>
    public static int av_image_fill_arrays(ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesize, byte* @src, AVPixelFormat @pix_fmt, int @width, int @height, int @align) => vectors.av_image_fill_arrays(ref @dst_data, ref @dst_linesize, @src, @pix_fmt, @width, @height, @align);
    
    /// <summary>Overwrite the image data with black. This is suitable for filling a sub-rectangle of an image, meaning the padding between the right most pixel and the left most pixel on the next line will not be overwritten. For some formats, the image size might be rounded up due to inherent alignment.</summary>
    /// <param name="dst_data">data pointers to destination image</param>
    /// <param name="dst_linesize">linesizes for the destination image</param>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="range">the color range of the image (important for colorspaces such as YUV)</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <returns>0 if the image data was cleared, a negative AVERROR code otherwise</returns>
    public static int av_image_fill_black(ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesize, AVPixelFormat @pix_fmt, AVColorRange @range, int @width, int @height) => vectors.av_image_fill_black(ref @dst_data, @dst_linesize, @pix_fmt, @range, @width, @height);
    
    /// <summary>Fill plane linesizes for an image with pixel format pix_fmt and width width.</summary>
    /// <param name="linesizes">array to be filled with the linesize for each plane</param>
    /// <returns>&gt;= 0 in case of success, a negative error code otherwise</returns>
    public static int av_image_fill_linesizes(ref int_array4 @linesizes, AVPixelFormat @pix_fmt, int @width) => vectors.av_image_fill_linesizes(ref @linesizes, @pix_fmt, @width);
    
    /// <summary>Compute the max pixel step for each plane of an image with a format described by pixdesc.</summary>
    /// <param name="max_pixsteps">an array which is filled with the max pixel step for each plane. Since a plane may contain different pixel components, the computed max_pixsteps[plane] is relative to the component in the plane with the max pixel step.</param>
    /// <param name="max_pixstep_comps">an array which is filled with the component for each plane which has the max pixel step. May be NULL.</param>
    public static void av_image_fill_max_pixsteps(ref int_array4 @max_pixsteps, ref int_array4 @max_pixstep_comps, AVPixFmtDescriptor* @pixdesc) => vectors.av_image_fill_max_pixsteps(ref @max_pixsteps, ref @max_pixstep_comps, @pixdesc);
    
    /// <summary>Fill plane sizes for an image with pixel format pix_fmt and height height.</summary>
    /// <param name="size">the array to be filled with the size of each image plane</param>
    /// <param name="linesizes">the array containing the linesize for each plane, should be filled by av_image_fill_linesizes()</param>
    /// <returns>&gt;= 0 in case of success, a negative error code otherwise</returns>
    public static int av_image_fill_plane_sizes(ref ulong_array4 @size, AVPixelFormat @pix_fmt, int @height, in long_array4 @linesizes) => vectors.av_image_fill_plane_sizes(ref @size, @pix_fmt, @height, @linesizes);
    
    /// <summary>Fill plane data pointers for an image with pixel format pix_fmt and height height.</summary>
    /// <param name="data">pointers array to be filled with the pointer for each image plane</param>
    /// <param name="ptr">the pointer to a buffer which will contain the image</param>
    /// <param name="linesizes">the array containing the linesize for each plane, should be filled by av_image_fill_linesizes()</param>
    /// <returns>the size in bytes required for the image buffer, a negative error code in case of failure</returns>
    public static int av_image_fill_pointers(ref byte_ptrArray4 @data, AVPixelFormat @pix_fmt, int @height, byte* @ptr, in int_array4 @linesizes) => vectors.av_image_fill_pointers(ref @data, @pix_fmt, @height, @ptr, @linesizes);
    
    /// <summary>Return the size in bytes of the amount of data required to store an image with the given parameters.</summary>
    /// <param name="pix_fmt">the pixel format of the image</param>
    /// <param name="width">the width of the image in pixels</param>
    /// <param name="height">the height of the image in pixels</param>
    /// <param name="align">the assumed linesize alignment</param>
    /// <returns>the buffer size in bytes, a negative error code in case of failure</returns>
    public static int av_image_get_buffer_size(AVPixelFormat @pix_fmt, int @width, int @height, int @align) => vectors.av_image_get_buffer_size(@pix_fmt, @width, @height, @align);
    
    /// <summary>Compute the size of an image line with format pix_fmt and width width for the plane plane.</summary>
    /// <returns>the computed size in bytes</returns>
    public static int av_image_get_linesize(AVPixelFormat @pix_fmt, int @width, int @plane) => vectors.av_image_get_linesize(@pix_fmt, @width, @plane);
    
    /// <summary>Get the index for a specific timestamp.</summary>
    /// <param name="st">stream that the timestamp belongs to</param>
    /// <param name="timestamp">timestamp to retrieve the index for</param>
    /// <param name="flags">if AVSEEK_FLAG_BACKWARD then the returned index will correspond to the timestamp which is &lt; = the requested one, if backward is 0, then it will be &gt;= if AVSEEK_FLAG_ANY seek to any frame, only keyframes otherwise</param>
    /// <returns>&lt; 0 if no such timestamp could be found</returns>
    public static int av_index_search_timestamp(AVStream* @st, long @timestamp, int @flags) => vectors.av_index_search_timestamp(@st, @timestamp, @flags);
    
    /// <summary>Initialize optional fields of a packet with default values.</summary>
    /// <param name="pkt">packet</param>
    [Obsolete("This function is deprecated. Once it's removed, sizeof(AVPacket) will not be a part of the ABI anymore.")]
    public static void av_init_packet(AVPacket* @pkt) => vectors.av_init_packet(@pkt);
    
    /// <summary>Audio input devices iterator.</summary>
    public static AVInputFormat* av_input_audio_device_next(AVInputFormat* @d) => vectors.av_input_audio_device_next(@d);
    
    /// <summary>Video input devices iterator.</summary>
    public static AVInputFormat* av_input_video_device_next(AVInputFormat* @d) => vectors.av_input_video_device_next(@d);
    
    /// <summary>Compute the length of an integer list.</summary>
    /// <param name="elsize">size in bytes of each list element (only 1, 2, 4 or 8)</param>
    /// <param name="list">pointer to the list</param>
    /// <param name="term">list terminator (usually 0 or -1)</param>
    /// <returns>length of the list, in elements, not counting the terminator</returns>
    public static uint av_int_list_length_for_size(uint @elsize, void* @list, ulong @term) => vectors.av_int_list_length_for_size(@elsize, @list, @term);
    
    /// <summary>Write a packet to an output media file ensuring correct interleaving.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="pkt">The packet containing the data to be written.  If the packet is reference-counted, this function will take ownership of this reference and unreference it later when it sees fit. If the packet is not reference-counted, libavformat will make a copy. The returned packet will be blank (as if returned from av_packet_alloc()), even on error.  This parameter can be NULL (at any time, not just at the end), to flush the interleaving queues.  Packet&apos;s &quot;stream_index&quot; field must be set to the index of the corresponding stream in &quot;s-&gt;streams&quot;.  The timestamps ( &quot;pts&quot;, &quot;dts&quot;) must be set to correct values in the stream&apos;s timebase (unless the output format is flagged with the AVFMT_NOTIMESTAMPS flag, then they can be set to AV_NOPTS_VALUE). The dts for subsequent packets in one stream must be strictly increasing (unless the output format is flagged with the AVFMT_TS_NONSTRICT, then they merely have to be nondecreasing).  &quot;duration&quot; should also be set if known.</param>
    /// <returns>0 on success, a negative AVERROR on error.</returns>
    public static int av_interleaved_write_frame(AVFormatContext* @s, AVPacket* @pkt) => vectors.av_interleaved_write_frame(@s, @pkt);
    
    /// <summary>Write an uncoded frame to an output media file.</summary>
    /// <returns>&gt;=0 for success, a negative code on error</returns>
    public static int av_interleaved_write_uncoded_frame(AVFormatContext* @s, int @stream_index, AVFrame* @frame) => vectors.av_interleaved_write_uncoded_frame(@s, @stream_index, @frame);
    
    /// <summary>Send the specified message to the log if the level is less than or equal to the current av_log_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct or NULL if general log.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    public static void av_log(void* @avcl, int @level, string @fmt) => vectors.av_log(@avcl, @level, @fmt);
    
    /// <summary>Default logging callback</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    /// <param name="vl">The arguments referenced by the format string.</param>
    public static void av_log_default_callback(void* @avcl, int @level, string @fmt, byte* @vl) => vectors.av_log_default_callback(@avcl, @level, @fmt, @vl);
    
    /// <summary>Format a line of log the same way as the default callback.</summary>
    /// <param name="line">buffer to receive the formatted line</param>
    /// <param name="line_size">size of the buffer</param>
    /// <param name="print_prefix">used to store whether the prefix must be printed; must point to a persistent integer initially set to 1</param>
    public static void av_log_format_line(void* @ptr, int @level, string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix) => vectors.av_log_format_line(@ptr, @level, @fmt, @vl, @line, @line_size, @print_prefix);
    
    /// <summary>Format a line of log the same way as the default callback.</summary>
    /// <param name="line">buffer to receive the formatted line; may be NULL if line_size is 0</param>
    /// <param name="line_size">size of the buffer; at most line_size-1 characters will be written to the buffer, plus one null terminator</param>
    /// <param name="print_prefix">used to store whether the prefix must be printed; must point to a persistent integer initially set to 1</param>
    /// <returns>Returns a negative value if an error occurred, otherwise returns the number of characters that would have been written for a sufficiently large buffer, not including the terminating null character. If the return value is not less than line_size, it means that the log message was truncated to fit the buffer.</returns>
    public static int av_log_format_line2(void* @ptr, int @level, string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix) => vectors.av_log_format_line2(@ptr, @level, @fmt, @vl, @line, @line_size, @print_prefix);
    
    public static int av_log_get_flags() => vectors.av_log_get_flags();
    
    /// <summary>Get the current log level</summary>
    /// <returns>Current log level</returns>
    public static int av_log_get_level() => vectors.av_log_get_level();
    
    /// <summary>Send the specified message to the log once with the initial_level and then with the subsequent_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct or NULL if general log.</param>
    /// <param name="initial_level">importance level of the message expressed using a &quot;Logging Constant&quot; for the first occurance.</param>
    /// <param name="subsequent_level">importance level of the message expressed using a &quot;Logging Constant&quot; after the first occurance.</param>
    /// <param name="state">a variable to keep trak of if a message has already been printed this must be initialized to 0 before the first use. The same state must not be accessed by 2 Threads simultaneously.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    public static void av_log_once(void* @avcl, int @initial_level, int @subsequent_level, int* @state, string @fmt) => vectors.av_log_once(@avcl, @initial_level, @subsequent_level, @state, @fmt);
    
    /// <summary>Set the logging callback</summary>
    /// <param name="callback">A logging function with a compatible signature.</param>
    public static void av_log_set_callback(av_log_set_callback_callback_func @callback) => vectors.av_log_set_callback(@callback);
    
    public static void av_log_set_flags(int @arg) => vectors.av_log_set_flags(@arg);
    
    /// <summary>Set the log level</summary>
    /// <param name="level">Logging level</param>
    public static void av_log_set_level(int @level) => vectors.av_log_set_level(@level);
    
    public static int av_log2(uint @v) => vectors.av_log2(@v);
    
    public static int av_log2_16bit(uint @v) => vectors.av_log2_16bit(@v);
    
    /// <summary>Allocate a memory block with alignment suitable for all memory accesses (including vectors if available on the CPU).</summary>
    /// <param name="size">Size in bytes for the memory block to be allocated</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    public static void* av_malloc(ulong @size) => vectors.av_malloc(@size);
    
    /// <summary>Allocate a memory block for an array with av_malloc().</summary>
    /// <param name="nmemb">Number of element</param>
    /// <param name="size">Size of a single element</param>
    /// <returns>Pointer to the allocated block, or `NULL` if the block cannot be allocated</returns>
    public static void* av_malloc_array(ulong @nmemb, ulong @size) => vectors.av_malloc_array(@nmemb, @size);
    
    /// <summary>Allocate a memory block with alignment suitable for all memory accesses (including vectors if available on the CPU) and zero all the bytes of the block.</summary>
    /// <param name="size">Size in bytes for the memory block to be allocated</param>
    /// <returns>Pointer to the allocated block, or `NULL` if it cannot be allocated</returns>
    public static void* av_mallocz(ulong @size) => vectors.av_mallocz(@size);
    
    [Obsolete("use av_calloc()")]
    public static void* av_mallocz_array(ulong @nmemb, ulong @size) => vectors.av_mallocz_array(@nmemb, @size);
    
    /// <summary>Allocate an AVMasteringDisplayMetadata structure and set its fields to default values. The resulting struct can be freed using av_freep().</summary>
    /// <returns>An AVMasteringDisplayMetadata filled with default values or NULL on failure.</returns>
    public static AVMasteringDisplayMetadata* av_mastering_display_metadata_alloc() => vectors.av_mastering_display_metadata_alloc();
    
    /// <summary>Allocate a complete AVMasteringDisplayMetadata and add it to the frame.</summary>
    /// <param name="frame">The frame which side data is added to.</param>
    /// <returns>The AVMasteringDisplayMetadata structure to be filled by caller.</returns>
    public static AVMasteringDisplayMetadata* av_mastering_display_metadata_create_side_data(AVFrame* @frame) => vectors.av_mastering_display_metadata_create_side_data(@frame);
    
    /// <summary>Return a positive value if the given filename has one of the given extensions, 0 otherwise.</summary>
    /// <param name="filename">file name to check against the given extensions</param>
    /// <param name="extensions">a comma-separated list of filename extensions</param>
    public static int av_match_ext(string @filename, string @extensions) => vectors.av_match_ext(@filename, @extensions);
    
    /// <summary>Set the maximum size that may be allocated in one block.</summary>
    /// <param name="max">Value to be set as the new maximum size</param>
    public static void av_max_alloc(ulong @max) => vectors.av_max_alloc(@max);
    
    /// <summary>Overlapping memcpy() implementation.</summary>
    /// <param name="dst">Destination buffer</param>
    /// <param name="back">Number of bytes back to start copying (i.e. the initial size of the overlapping window); must be &gt; 0</param>
    /// <param name="cnt">Number of bytes to copy; must be &gt;= 0</param>
    public static void av_memcpy_backptr(byte* @dst, int @back, int @cnt) => vectors.av_memcpy_backptr(@dst, @back, @cnt);
    
    /// <summary>Duplicate a buffer with av_malloc().</summary>
    /// <param name="p">Buffer to be duplicated</param>
    /// <param name="size">Size in bytes of the buffer copied</param>
    /// <returns>Pointer to a newly allocated buffer containing a copy of `p` or `NULL` if the buffer cannot be allocated</returns>
    public static void* av_memdup(void* @p, ulong @size) => vectors.av_memdup(@p, @size);
    
    /// <summary>Multiply two rationals.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b*c</returns>
    public static AVRational av_mul_q(AVRational @b, AVRational @c) => vectors.av_mul_q(@b, @c);
    
    /// <summary>Iterate over all registered muxers.</summary>
    /// <param name="opaque">a pointer where libavformat will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered muxer or NULL when the iteration is finished</returns>
    public static AVOutputFormat* av_muxer_iterate(void** @opaque) => vectors.av_muxer_iterate(@opaque);
    
    /// <summary>Find which of the two rationals is closer to another rational.</summary>
    /// <param name="q">Rational to be compared against</param>
    /// <returns>One of the following values: - 1 if `q1` is nearer to `q` than `q2` - -1 if `q2` is nearer to `q` than `q1` - 0 if they have the same distance</returns>
    public static int av_nearer_q(AVRational @q, AVRational @q1, AVRational @q2) => vectors.av_nearer_q(@q, @q1, @q2);
    
    /// <summary>Allocate the payload of a packet and initialize its fields with default values.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="size">wanted payload size</param>
    /// <returns>0 if OK, AVERROR_xxx otherwise</returns>
    public static int av_new_packet(AVPacket* @pkt, int @size) => vectors.av_new_packet(@pkt, @size);
    
    public static AVProgram* av_new_program(AVFormatContext* @s, int @id) => vectors.av_new_program(@s, @id);
    
    /// <summary>Iterate over potential AVOptions-enabled children of parent.</summary>
    /// <param name="iter">a pointer where iteration state is stored.</param>
    /// <returns>AVClass corresponding to next potential child or NULL</returns>
    public static AVClass* av_opt_child_class_iterate(AVClass* @parent, void** @iter) => vectors.av_opt_child_class_iterate(@parent, @iter);
    
    /// <summary>Iterate over AVOptions-enabled children of obj.</summary>
    /// <param name="prev">result of a previous call to this function or NULL</param>
    /// <returns>next AVOptions-enabled child or NULL</returns>
    public static void* av_opt_child_next(void* @obj, void* @prev) => vectors.av_opt_child_next(@obj, @prev);
    
    /// <summary>Copy options from src object into dest object.</summary>
    /// <param name="dest">Object to copy from</param>
    /// <param name="src">Object to copy into</param>
    /// <returns>0 on success, negative on error</returns>
    public static int av_opt_copy(void* @dest, void* @src) => vectors.av_opt_copy(@dest, @src);
    
    public static int av_opt_eval_double(void* @obj, AVOption* @o, string @val, double* @double_out) => vectors.av_opt_eval_double(@obj, @o, @val, @double_out);
    
    /// <summary>@{ This group of functions can be used to evaluate option strings and get numbers out of them. They do the same thing as av_opt_set(), except the result is written into the caller-supplied pointer.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass.</param>
    /// <param name="o">an option for which the string is to be evaluated.</param>
    /// <param name="val">string to be evaluated.</param>
    /// <returns>0 on success, a negative number on failure.</returns>
    public static int av_opt_eval_flags(void* @obj, AVOption* @o, string @val, int* @flags_out) => vectors.av_opt_eval_flags(@obj, @o, @val, @flags_out);
    
    public static int av_opt_eval_float(void* @obj, AVOption* @o, string @val, float* @float_out) => vectors.av_opt_eval_float(@obj, @o, @val, @float_out);
    
    public static int av_opt_eval_int(void* @obj, AVOption* @o, string @val, int* @int_out) => vectors.av_opt_eval_int(@obj, @o, @val, @int_out);
    
    public static int av_opt_eval_int64(void* @obj, AVOption* @o, string @val, long* @int64_out) => vectors.av_opt_eval_int64(@obj, @o, @val, @int64_out);
    
    public static int av_opt_eval_q(void* @obj, AVOption* @o, string @val, AVRational* @q_out) => vectors.av_opt_eval_q(@obj, @o, @val, @q_out);
    
    /// <summary>Look for an option in an object. Consider only options which have all the specified flags set.</summary>
    /// <param name="obj">A pointer to a struct whose first element is a pointer to an AVClass. Alternatively a double pointer to an AVClass, if AV_OPT_SEARCH_FAKE_OBJ search flag is set.</param>
    /// <param name="name">The name of the option to look for.</param>
    /// <param name="unit">When searching for named constants, name of the unit it belongs to.</param>
    /// <param name="opt_flags">Find only options with all the specified flags set (AV_OPT_FLAG).</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <returns>A pointer to the option found, or NULL if no option was found.</returns>
    public static AVOption* av_opt_find(void* @obj, string @name, string @unit, int @opt_flags, int @search_flags) => vectors.av_opt_find(@obj, @name, @unit, @opt_flags, @search_flags);
    
    /// <summary>Look for an option in an object. Consider only options which have all the specified flags set.</summary>
    /// <param name="obj">A pointer to a struct whose first element is a pointer to an AVClass. Alternatively a double pointer to an AVClass, if AV_OPT_SEARCH_FAKE_OBJ search flag is set.</param>
    /// <param name="name">The name of the option to look for.</param>
    /// <param name="unit">When searching for named constants, name of the unit it belongs to.</param>
    /// <param name="opt_flags">Find only options with all the specified flags set (AV_OPT_FLAG).</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <param name="target_obj">if non-NULL, an object to which the option belongs will be written here. It may be different from obj if AV_OPT_SEARCH_CHILDREN is present in search_flags. This parameter is ignored if search_flags contain AV_OPT_SEARCH_FAKE_OBJ.</param>
    /// <returns>A pointer to the option found, or NULL if no option was found.</returns>
    public static AVOption* av_opt_find2(void* @obj, string @name, string @unit, int @opt_flags, int @search_flags, void** @target_obj) => vectors.av_opt_find2(@obj, @name, @unit, @opt_flags, @search_flags, @target_obj);
    
    /// <summary>Check whether a particular flag is set in a flags field.</summary>
    /// <param name="field_name">the name of the flag field option</param>
    /// <param name="flag_name">the name of the flag to check</param>
    /// <returns>non-zero if the flag is set, zero if the flag isn&apos;t set, isn&apos;t of the right type, or the flags field doesn&apos;t exist.</returns>
    public static int av_opt_flag_is_set(void* @obj, string @field_name, string @flag_name) => vectors.av_opt_flag_is_set(@obj, @field_name, @flag_name);
    
    /// <summary>Free all allocated objects in obj.</summary>
    public static void av_opt_free(void* @obj) => vectors.av_opt_free(@obj);
    
    /// <summary>Free an AVOptionRanges struct and set it to NULL.</summary>
    public static void av_opt_freep_ranges(AVOptionRanges** @ranges) => vectors.av_opt_freep_ranges(@ranges);
    
    /// <summary>@{ Those functions get a value of the option with the given name from an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to an AVClass.</param>
    /// <param name="name">name of the option to get.</param>
    /// <param name="search_flags">flags passed to av_opt_find2. I.e. if AV_OPT_SEARCH_CHILDREN is passed here, then the option may be found in a child of obj.</param>
    /// <param name="out_val">value of the option will be written here</param>
    /// <returns>&gt;=0 on success, a negative error code otherwise</returns>
    public static int av_opt_get(void* @obj, string @name, int @search_flags, byte** @out_val) => vectors.av_opt_get(@obj, @name, @search_flags, @out_val);
    
    [Obsolete()]
    public static int av_opt_get_channel_layout(void* @obj, string @name, int @search_flags, long* @ch_layout) => vectors.av_opt_get_channel_layout(@obj, @name, @search_flags, @ch_layout);
    
    public static int av_opt_get_chlayout(void* @obj, string @name, int @search_flags, AVChannelLayout* @layout) => vectors.av_opt_get_chlayout(@obj, @name, @search_flags, @layout);
    
    /// <param name="out_val">The returned dictionary is a copy of the actual value and must be freed with av_dict_free() by the caller</param>
    public static int av_opt_get_dict_val(void* @obj, string @name, int @search_flags, AVDictionary** @out_val) => vectors.av_opt_get_dict_val(@obj, @name, @search_flags, @out_val);
    
    public static int av_opt_get_double(void* @obj, string @name, int @search_flags, double* @out_val) => vectors.av_opt_get_double(@obj, @name, @search_flags, @out_val);
    
    public static int av_opt_get_image_size(void* @obj, string @name, int @search_flags, int* @w_out, int* @h_out) => vectors.av_opt_get_image_size(@obj, @name, @search_flags, @w_out, @h_out);
    
    public static int av_opt_get_int(void* @obj, string @name, int @search_flags, long* @out_val) => vectors.av_opt_get_int(@obj, @name, @search_flags, @out_val);
    
    /// <summary>Extract a key-value pair from the beginning of a string.</summary>
    /// <param name="ropts">pointer to the options string, will be updated to point to the rest of the string (one of the pairs_sep or the final NUL)</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value, for example &apos;=&apos;</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other, for example &apos;:&apos; or &apos;,&apos;</param>
    /// <param name="flags">flags; see the AV_OPT_FLAG_* values below</param>
    /// <param name="rkey">parsed key; must be freed using av_free()</param>
    /// <param name="rval">parsed value; must be freed using av_free()</param>
    /// <returns>&gt;=0 for success, or a negative value corresponding to an AVERROR code in case of error; in particular: AVERROR(EINVAL) if no key is present</returns>
    public static int av_opt_get_key_value(byte** @ropts, string @key_val_sep, string @pairs_sep, uint @flags, byte** @rkey, byte** @rval) => vectors.av_opt_get_key_value(@ropts, @key_val_sep, @pairs_sep, @flags, @rkey, @rval);
    
    public static int av_opt_get_pixel_fmt(void* @obj, string @name, int @search_flags, AVPixelFormat* @out_fmt) => vectors.av_opt_get_pixel_fmt(@obj, @name, @search_flags, @out_fmt);
    
    public static int av_opt_get_q(void* @obj, string @name, int @search_flags, AVRational* @out_val) => vectors.av_opt_get_q(@obj, @name, @search_flags, @out_val);
    
    public static int av_opt_get_sample_fmt(void* @obj, string @name, int @search_flags, AVSampleFormat* @out_fmt) => vectors.av_opt_get_sample_fmt(@obj, @name, @search_flags, @out_fmt);
    
    public static int av_opt_get_video_rate(void* @obj, string @name, int @search_flags, AVRational* @out_val) => vectors.av_opt_get_video_rate(@obj, @name, @search_flags, @out_val);
    
    /// <summary>Check if given option is set to its default value.</summary>
    /// <param name="obj">AVClass object to check option on</param>
    /// <param name="o">option to be checked</param>
    /// <returns>&gt;0 when option is set to its default, 0 when option is not set its default,  &lt; 0 on error</returns>
    public static int av_opt_is_set_to_default(void* @obj, AVOption* @o) => vectors.av_opt_is_set_to_default(@obj, @o);
    
    /// <summary>Check if given option is set to its default value.</summary>
    /// <param name="obj">AVClass object to check option on</param>
    /// <param name="name">option name</param>
    /// <param name="search_flags">combination of AV_OPT_SEARCH_*</param>
    /// <returns>&gt;0 when option is set to its default, 0 when option is not set its default,  &lt; 0 on error</returns>
    public static int av_opt_is_set_to_default_by_name(void* @obj, string @name, int @search_flags) => vectors.av_opt_is_set_to_default_by_name(@obj, @name, @search_flags);
    
    /// <summary>Iterate over all AVOptions belonging to obj.</summary>
    /// <param name="obj">an AVOptions-enabled struct or a double pointer to an AVClass describing it.</param>
    /// <param name="prev">result of the previous call to av_opt_next() on this object or NULL</param>
    /// <returns>next AVOption or NULL</returns>
    public static AVOption* av_opt_next(void* @obj, AVOption* @prev) => vectors.av_opt_next(@obj, @prev);
    
    /// <summary>@}</summary>
    public static void* av_opt_ptr(AVClass* @avclass, void* @obj, string @name) => vectors.av_opt_ptr(@avclass, @obj, @name);
    
    /// <summary>Get a list of allowed ranges for the given option.</summary>
    /// <param name="flags">is a bitmask of flags, undefined flags should not be set and should be ignored AV_OPT_SEARCH_FAKE_OBJ indicates that the obj is a double pointer to a AVClass instead of a full instance AV_OPT_MULTI_COMPONENT_RANGE indicates that function may return more than one component,</param>
    /// <returns>number of compontents returned on success, a negative errro code otherwise</returns>
    public static int av_opt_query_ranges(AVOptionRanges** @p0, void* @obj, string @key, int @flags) => vectors.av_opt_query_ranges(@p0, @obj, @key, @flags);
    
    /// <summary>Get a default list of allowed ranges for the given option.</summary>
    /// <param name="flags">is a bitmask of flags, undefined flags should not be set and should be ignored AV_OPT_SEARCH_FAKE_OBJ indicates that the obj is a double pointer to a AVClass instead of a full instance AV_OPT_MULTI_COMPONENT_RANGE indicates that function may return more than one component,</param>
    /// <returns>number of compontents returned on success, a negative errro code otherwise</returns>
    public static int av_opt_query_ranges_default(AVOptionRanges** @p0, void* @obj, string @key, int @flags) => vectors.av_opt_query_ranges_default(@p0, @obj, @key, @flags);
    
    /// <summary>Serialize object&apos;s options.</summary>
    /// <param name="obj">AVClass object to serialize</param>
    /// <param name="opt_flags">serialize options with all the specified flags set (AV_OPT_FLAG)</param>
    /// <param name="flags">combination of AV_OPT_SERIALIZE_* flags</param>
    /// <param name="buffer">Pointer to buffer that will be allocated with string containg serialized options. Buffer must be freed by the caller when is no longer needed.</param>
    /// <param name="key_val_sep">character used to separate key from value</param>
    /// <param name="pairs_sep">character used to separate two pairs from each other</param>
    /// <returns>&gt;= 0 on success, negative on error</returns>
    public static int av_opt_serialize(void* @obj, int @opt_flags, int @flags, byte** @buffer, byte @key_val_sep, byte @pairs_sep) => vectors.av_opt_serialize(@obj, @opt_flags, @flags, @buffer, @key_val_sep, @pairs_sep);
    
    /// <summary>@{ Those functions set the field of obj with the given name to value.</summary>
    /// <param name="obj">A struct whose first element is a pointer to an AVClass.</param>
    /// <param name="name">the name of the field to set</param>
    /// <param name="val">The value to set. In case of av_opt_set() if the field is not of a string type, then the given string is parsed. SI postfixes and some named scalars are supported. If the field is of a numeric type, it has to be a numeric or named scalar. Behavior with more than one scalar and +- infix operators is undefined. If the field is of a flags type, it has to be a sequence of numeric scalars or named flags separated by &apos;+&apos; or &apos;-&apos;. Prefixing a flag with &apos;+&apos; causes it to be set without affecting the other flags; similarly, &apos;-&apos; unsets a flag. If the field is of a dictionary type, it has to be a &apos;:&apos; separated list of key=value parameters. Values containing &apos;:&apos; special characters must be escaped.</param>
    /// <param name="search_flags">flags passed to av_opt_find2. I.e. if AV_OPT_SEARCH_CHILDREN is passed here, then the option may be set on a child of obj.</param>
    /// <returns>0 if the value has been set, or an AVERROR code in case of error: AVERROR_OPTION_NOT_FOUND if no matching option exists AVERROR(ERANGE) if the value is out of range AVERROR(EINVAL) if the value is not valid</returns>
    public static int av_opt_set(void* @obj, string @name, string @val, int @search_flags) => vectors.av_opt_set(@obj, @name, @val, @search_flags);
    
    public static int av_opt_set_bin(void* @obj, string @name, byte* @val, int @size, int @search_flags) => vectors.av_opt_set_bin(@obj, @name, @val, @size, @search_flags);
    
    [Obsolete()]
    public static int av_opt_set_channel_layout(void* @obj, string @name, long @ch_layout, int @search_flags) => vectors.av_opt_set_channel_layout(@obj, @name, @ch_layout, @search_flags);
    
    public static int av_opt_set_chlayout(void* @obj, string @name, AVChannelLayout* @layout, int @search_flags) => vectors.av_opt_set_chlayout(@obj, @name, @layout, @search_flags);
    
    /// <summary>Set the values of all AVOption fields to their default values.</summary>
    /// <param name="s">an AVOption-enabled struct (its first member must be a pointer to AVClass)</param>
    public static void av_opt_set_defaults(void* @s) => vectors.av_opt_set_defaults(@s);
    
    /// <summary>Set the values of all AVOption fields to their default values. Only these AVOption fields for which (opt-&gt;flags &amp; mask) == flags will have their default applied to s.</summary>
    /// <param name="s">an AVOption-enabled struct (its first member must be a pointer to AVClass)</param>
    /// <param name="mask">combination of AV_OPT_FLAG_*</param>
    /// <param name="flags">combination of AV_OPT_FLAG_*</param>
    public static void av_opt_set_defaults2(void* @s, int @mask, int @flags) => vectors.av_opt_set_defaults2(@s, @mask, @flags);
    
    /// <summary>Set all the options from a given dictionary on an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass</param>
    /// <param name="options">options to process. This dictionary will be freed and replaced by a new one containing all options not found in obj. Of course this new dictionary needs to be freed by caller with av_dict_free().</param>
    /// <returns>0 on success, a negative AVERROR if some option was found in obj, but could not be set.</returns>
    public static int av_opt_set_dict(void* @obj, AVDictionary** @options) => vectors.av_opt_set_dict(@obj, @options);
    
    public static int av_opt_set_dict_val(void* @obj, string @name, AVDictionary* @val, int @search_flags) => vectors.av_opt_set_dict_val(@obj, @name, @val, @search_flags);
    
    /// <summary>Set all the options from a given dictionary on an object.</summary>
    /// <param name="obj">a struct whose first element is a pointer to AVClass</param>
    /// <param name="options">options to process. This dictionary will be freed and replaced by a new one containing all options not found in obj. Of course this new dictionary needs to be freed by caller with av_dict_free().</param>
    /// <param name="search_flags">A combination of AV_OPT_SEARCH_*.</param>
    /// <returns>0 on success, a negative AVERROR if some option was found in obj, but could not be set.</returns>
    public static int av_opt_set_dict2(void* @obj, AVDictionary** @options, int @search_flags) => vectors.av_opt_set_dict2(@obj, @options, @search_flags);
    
    public static int av_opt_set_double(void* @obj, string @name, double @val, int @search_flags) => vectors.av_opt_set_double(@obj, @name, @val, @search_flags);
    
    /// <summary>Parse the key-value pairs list in opts. For each key=value pair found, set the value of the corresponding option in ctx.</summary>
    /// <param name="ctx">the AVClass object to set options on</param>
    /// <param name="opts">the options string, key-value pairs separated by a delimiter</param>
    /// <param name="shorthand">a NULL-terminated array of options names for shorthand notation: if the first field in opts has no key part, the key is taken from the first element of shorthand; then again for the second, etc., until either opts is finished, shorthand is finished or a named option is found; after that, all options must be named</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value, for example &apos;=&apos;</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other, for example &apos;:&apos; or &apos;,&apos;</param>
    /// <returns>the number of successfully set key=value pairs, or a negative value corresponding to an AVERROR code in case of error: AVERROR(EINVAL) if opts cannot be parsed, the error code issued by av_set_string3() if a key/value pair cannot be set</returns>
    public static int av_opt_set_from_string(void* @ctx, string @opts, byte** @shorthand, string @key_val_sep, string @pairs_sep) => vectors.av_opt_set_from_string(@ctx, @opts, @shorthand, @key_val_sep, @pairs_sep);
    
    public static int av_opt_set_image_size(void* @obj, string @name, int @w, int @h, int @search_flags) => vectors.av_opt_set_image_size(@obj, @name, @w, @h, @search_flags);
    
    public static int av_opt_set_int(void* @obj, string @name, long @val, int @search_flags) => vectors.av_opt_set_int(@obj, @name, @val, @search_flags);
    
    public static int av_opt_set_pixel_fmt(void* @obj, string @name, AVPixelFormat @fmt, int @search_flags) => vectors.av_opt_set_pixel_fmt(@obj, @name, @fmt, @search_flags);
    
    public static int av_opt_set_q(void* @obj, string @name, AVRational @val, int @search_flags) => vectors.av_opt_set_q(@obj, @name, @val, @search_flags);
    
    public static int av_opt_set_sample_fmt(void* @obj, string @name, AVSampleFormat @fmt, int @search_flags) => vectors.av_opt_set_sample_fmt(@obj, @name, @fmt, @search_flags);
    
    public static int av_opt_set_video_rate(void* @obj, string @name, AVRational @val, int @search_flags) => vectors.av_opt_set_video_rate(@obj, @name, @val, @search_flags);
    
    /// <summary>Show the obj options.</summary>
    /// <param name="av_log_obj">log context to use for showing the options</param>
    /// <param name="req_flags">requested flags for the options to show. Show only the options for which it is opt-&gt;flags &amp; req_flags.</param>
    /// <param name="rej_flags">rejected flags for the options to show. Show only the options for which it is !(opt-&gt;flags &amp; req_flags).</param>
    public static int av_opt_show2(void* @obj, void* @av_log_obj, int @req_flags, int @rej_flags) => vectors.av_opt_show2(@obj, @av_log_obj, @req_flags, @rej_flags);
    
    /// <summary>Audio output devices iterator.</summary>
    public static AVOutputFormat* av_output_audio_device_next(AVOutputFormat* @d) => vectors.av_output_audio_device_next(@d);
    
    /// <summary>Video output devices iterator.</summary>
    public static AVOutputFormat* av_output_video_device_next(AVOutputFormat* @d) => vectors.av_output_video_device_next(@d);
    
    /// <summary>Wrap an existing array as a packet side data.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="data">the side data array. It must be allocated with the av_malloc() family of functions. The ownership of the data is transferred to pkt.</param>
    /// <param name="size">side information size</param>
    /// <returns>a non-negative number on success, a negative AVERROR code on failure. On failure, the packet is unchanged and the data remains owned by the caller.</returns>
    public static int av_packet_add_side_data(AVPacket* @pkt, AVPacketSideDataType @type, byte* @data, ulong @size) => vectors.av_packet_add_side_data(@pkt, @type, @data, @size);
    
    /// <summary>Allocate an AVPacket and set its fields to default values. The resulting struct must be freed using av_packet_free().</summary>
    /// <returns>An AVPacket filled with default values or NULL on failure.</returns>
    public static AVPacket* av_packet_alloc() => vectors.av_packet_alloc();
    
    /// <summary>Create a new packet that references the same data as src.</summary>
    /// <returns>newly created AVPacket on success, NULL on error.</returns>
    public static AVPacket* av_packet_clone(AVPacket* @src) => vectors.av_packet_clone(@src);
    
    /// <summary>Copy only &quot;properties&quot; fields from src to dst.</summary>
    /// <param name="dst">Destination packet</param>
    /// <param name="src">Source packet</param>
    /// <returns>0 on success AVERROR on failure.</returns>
    public static int av_packet_copy_props(AVPacket* @dst, AVPacket* @src) => vectors.av_packet_copy_props(@dst, @src);
    
    /// <summary>Free the packet, if the packet is reference counted, it will be unreferenced first.</summary>
    /// <param name="pkt">packet to be freed. The pointer will be set to NULL.</param>
    public static void av_packet_free(AVPacket** @pkt) => vectors.av_packet_free(@pkt);
    
    /// <summary>Convenience function to free all the side data stored. All the other fields stay untouched.</summary>
    /// <param name="pkt">packet</param>
    public static void av_packet_free_side_data(AVPacket* @pkt) => vectors.av_packet_free_side_data(@pkt);
    
    /// <summary>Initialize a reference-counted packet from av_malloc()ed data.</summary>
    /// <param name="pkt">packet to be initialized. This function will set the data, size, and buf fields, all others are left untouched.</param>
    /// <param name="data">Data allocated by av_malloc() to be used as packet data. If this function returns successfully, the data is owned by the underlying AVBuffer. The caller may not access the data through other means.</param>
    /// <param name="size">size of data in bytes, without the padding. I.e. the full buffer size is assumed to be size + AV_INPUT_BUFFER_PADDING_SIZE.</param>
    /// <returns>0 on success, a negative AVERROR on error</returns>
    public static int av_packet_from_data(AVPacket* @pkt, byte* @data, int @size) => vectors.av_packet_from_data(@pkt, @data, @size);
    
    /// <summary>Get side information from packet.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">If supplied, *size will be set to the size of the side data or to zero if the desired side data is not present.</param>
    /// <returns>pointer to data if present or NULL otherwise</returns>
    public static byte* av_packet_get_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong* @size) => vectors.av_packet_get_side_data(@pkt, @type, @size);
    
    /// <summary>Ensure the data described by a given packet is reference counted.</summary>
    /// <param name="pkt">packet whose data should be made reference counted.</param>
    /// <returns>0 on success, a negative AVERROR on error. On failure, the packet is unchanged.</returns>
    public static int av_packet_make_refcounted(AVPacket* @pkt) => vectors.av_packet_make_refcounted(@pkt);
    
    /// <summary>Create a writable reference for the data described by a given packet, avoiding data copy if possible.</summary>
    /// <param name="pkt">Packet whose data should be made writable.</param>
    /// <returns>0 on success, a negative AVERROR on failure. On failure, the packet is unchanged.</returns>
    public static int av_packet_make_writable(AVPacket* @pkt) => vectors.av_packet_make_writable(@pkt);
    
    /// <summary>Move every field in src to dst and reset src.</summary>
    /// <param name="dst">Destination packet</param>
    /// <param name="src">Source packet, will be reset</param>
    public static void av_packet_move_ref(AVPacket* @dst, AVPacket* @src) => vectors.av_packet_move_ref(@dst, @src);
    
    /// <summary>Allocate new information of a packet.</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="size">side information size</param>
    /// <returns>pointer to fresh allocated data or NULL otherwise</returns>
    public static byte* av_packet_new_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size) => vectors.av_packet_new_side_data(@pkt, @type, @size);
    
    /// <summary>Pack a dictionary for use in side_data.</summary>
    /// <param name="dict">The dictionary to pack.</param>
    /// <param name="size">pointer to store the size of the returned data</param>
    /// <returns>pointer to data if successful, NULL otherwise</returns>
    public static byte* av_packet_pack_dictionary(AVDictionary* @dict, ulong* @size) => vectors.av_packet_pack_dictionary(@dict, @size);
    
    /// <summary>Setup a new reference to the data described by a given packet</summary>
    /// <param name="dst">Destination packet. Will be completely overwritten.</param>
    /// <param name="src">Source packet</param>
    /// <returns>0 on success, a negative AVERROR on error. On error, dst will be blank (as if returned by av_packet_alloc()).</returns>
    public static int av_packet_ref(AVPacket* @dst, AVPacket* @src) => vectors.av_packet_ref(@dst, @src);
    
    /// <summary>Convert valid timing fields (timestamps / durations) in a packet from one timebase to another. Timestamps with unknown values (AV_NOPTS_VALUE) will be ignored.</summary>
    /// <param name="pkt">packet on which the conversion will be performed</param>
    /// <param name="tb_src">source timebase, in which the timing fields in pkt are expressed</param>
    /// <param name="tb_dst">destination timebase, to which the timing fields will be converted</param>
    public static void av_packet_rescale_ts(AVPacket* @pkt, AVRational @tb_src, AVRational @tb_dst) => vectors.av_packet_rescale_ts(@pkt, @tb_src, @tb_dst);
    
    /// <summary>Shrink the already allocated side data buffer</summary>
    /// <param name="pkt">packet</param>
    /// <param name="type">side information type</param>
    /// <param name="size">new side information size</param>
    /// <returns>0 on success, &lt; 0 on failure</returns>
    public static int av_packet_shrink_side_data(AVPacket* @pkt, AVPacketSideDataType @type, ulong @size) => vectors.av_packet_shrink_side_data(@pkt, @type, @size);
    
    public static string av_packet_side_data_name(AVPacketSideDataType @type) => vectors.av_packet_side_data_name(@type);
    
    /// <summary>Unpack a dictionary from side_data.</summary>
    /// <param name="data">data from side_data</param>
    /// <param name="size">size of the data</param>
    /// <param name="dict">the metadata storage dictionary</param>
    /// <returns>0 on success, &lt; 0 on failure</returns>
    public static int av_packet_unpack_dictionary(byte* @data, ulong @size, AVDictionary** @dict) => vectors.av_packet_unpack_dictionary(@data, @size, @dict);
    
    /// <summary>Wipe the packet.</summary>
    /// <param name="pkt">The packet to be unreferenced.</param>
    public static void av_packet_unref(AVPacket* @pkt) => vectors.av_packet_unref(@pkt);
    
    /// <summary>Parse CPU caps from a string and update the given AV_CPU_* flags based on that.</summary>
    /// <returns>negative on error.</returns>
    public static int av_parse_cpu_caps(uint* @flags, string @s) => vectors.av_parse_cpu_caps(@flags, @s);
    
    public static void av_parser_close(AVCodecParserContext* @s) => vectors.av_parser_close(@s);
    
    public static AVCodecParserContext* av_parser_init(int @codec_id) => vectors.av_parser_init(@codec_id);
    
    /// <summary>Iterate over all registered codec parsers.</summary>
    /// <param name="opaque">a pointer where libavcodec will store the iteration state. Must point to NULL to start the iteration.</param>
    /// <returns>the next registered codec parser or NULL when the iteration is finished</returns>
    public static AVCodecParser* av_parser_iterate(void** @opaque) => vectors.av_parser_iterate(@opaque);
    
    /// <summary>Parse a packet.</summary>
    /// <param name="s">parser context.</param>
    /// <param name="avctx">codec context.</param>
    /// <param name="poutbuf">set to pointer to parsed buffer or NULL if not yet finished.</param>
    /// <param name="poutbuf_size">set to size of parsed buffer or zero if not yet finished.</param>
    /// <param name="buf">input buffer.</param>
    /// <param name="buf_size">buffer size in bytes without the padding. I.e. the full buffer size is assumed to be buf_size + AV_INPUT_BUFFER_PADDING_SIZE. To signal EOF, this should be 0 (so that the last frame can be output).</param>
    /// <param name="pts">input presentation timestamp.</param>
    /// <param name="dts">input decoding timestamp.</param>
    /// <param name="pos">input byte position in stream.</param>
    /// <returns>the number of bytes of the input bitstream used.</returns>
    public static int av_parser_parse2(AVCodecParserContext* @s, AVCodecContext* @avctx, byte** @poutbuf, int* @poutbuf_size, byte* @buf, int @buf_size, long @pts, long @dts, long @pos) => vectors.av_parser_parse2(@s, @avctx, @poutbuf, @poutbuf_size, @buf, @buf_size, @pts, @dts, @pos);
    
    /// <summary>Returns number of planes in pix_fmt, a negative AVERROR if pix_fmt is not a valid pixel format.</summary>
    /// <returns>number of planes in pix_fmt, a negative AVERROR if pix_fmt is not a valid pixel format.</returns>
    public static int av_pix_fmt_count_planes(AVPixelFormat @pix_fmt) => vectors.av_pix_fmt_count_planes(@pix_fmt);
    
    /// <summary>Returns a pixel format descriptor for provided pixel format or NULL if this pixel format is unknown.</summary>
    /// <returns>a pixel format descriptor for provided pixel format or NULL if this pixel format is unknown.</returns>
    public static AVPixFmtDescriptor* av_pix_fmt_desc_get(AVPixelFormat @pix_fmt) => vectors.av_pix_fmt_desc_get(@pix_fmt);
    
    /// <summary>Returns an AVPixelFormat id described by desc, or AV_PIX_FMT_NONE if desc is not a valid pointer to a pixel format descriptor.</summary>
    /// <returns>an AVPixelFormat id described by desc, or AV_PIX_FMT_NONE if desc is not a valid pointer to a pixel format descriptor.</returns>
    public static AVPixelFormat av_pix_fmt_desc_get_id(AVPixFmtDescriptor* @desc) => vectors.av_pix_fmt_desc_get_id(@desc);
    
    /// <summary>Iterate over all pixel format descriptors known to libavutil.</summary>
    /// <param name="prev">previous descriptor. NULL to get the first descriptor.</param>
    /// <returns>next descriptor or NULL after the last descriptor</returns>
    public static AVPixFmtDescriptor* av_pix_fmt_desc_next(AVPixFmtDescriptor* @prev) => vectors.av_pix_fmt_desc_next(@prev);
    
    /// <summary>Utility function to access log2_chroma_w log2_chroma_h from the pixel format AVPixFmtDescriptor.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <param name="h_shift">store log2_chroma_w (horizontal/width shift)</param>
    /// <param name="v_shift">store log2_chroma_h (vertical/height shift)</param>
    /// <returns>0 on success, AVERROR(ENOSYS) on invalid or unknown pixel format</returns>
    public static int av_pix_fmt_get_chroma_sub_sample(AVPixelFormat @pix_fmt, int* @h_shift, int* @v_shift) => vectors.av_pix_fmt_get_chroma_sub_sample(@pix_fmt, @h_shift, @v_shift);
    
    /// <summary>Utility function to swap the endianness of a pixel format.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <returns>pixel format with swapped endianness if it exists, otherwise AV_PIX_FMT_NONE</returns>
    public static AVPixelFormat av_pix_fmt_swap_endianness(AVPixelFormat @pix_fmt) => vectors.av_pix_fmt_swap_endianness(@pix_fmt);
    
    /// <summary>Send a nice dump of a packet to the log.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message, lower values signifying higher importance.</param>
    /// <param name="pkt">packet to dump</param>
    /// <param name="dump_payload">True if the payload must be displayed, too.</param>
    /// <param name="st">AVStream that the packet belongs to</param>
    public static void av_pkt_dump_log2(void* @avcl, int @level, AVPacket* @pkt, int @dump_payload, AVStream* @st) => vectors.av_pkt_dump_log2(@avcl, @level, @pkt, @dump_payload, @st);
    
    /// <summary>Send a nice dump of a packet to the specified file stream.</summary>
    /// <param name="f">The file stream pointer where the dump should be sent to.</param>
    /// <param name="pkt">packet to dump</param>
    /// <param name="dump_payload">True if the payload must be displayed, too.</param>
    /// <param name="st">AVStream that the packet belongs to</param>
    public static void av_pkt_dump2(_iobuf* @f, AVPacket* @pkt, int @dump_payload, AVStream* @st) => vectors.av_pkt_dump2(@f, @pkt, @dump_payload, @st);
    
    /// <summary>Like av_probe_input_buffer2() but returns 0 on success</summary>
    public static int av_probe_input_buffer(AVIOContext* @pb, AVInputFormat** @fmt, string @url, void* @logctx, uint @offset, uint @max_probe_size) => vectors.av_probe_input_buffer(@pb, @fmt, @url, @logctx, @offset, @max_probe_size);
    
    /// <summary>Probe a bytestream to determine the input format. Each time a probe returns with a score that is too low, the probe buffer size is increased and another attempt is made. When the maximum probe size is reached, the input format with the highest score is returned.</summary>
    /// <param name="pb">the bytestream to probe</param>
    /// <param name="fmt">the input format is put here</param>
    /// <param name="url">the url of the stream</param>
    /// <param name="logctx">the log context</param>
    /// <param name="offset">the offset within the bytestream to probe from</param>
    /// <param name="max_probe_size">the maximum probe buffer size (zero for default)</param>
    /// <returns>the score in case of success, a negative value corresponding to an the maximal score is AVPROBE_SCORE_MAX AVERROR code otherwise</returns>
    public static int av_probe_input_buffer2(AVIOContext* @pb, AVInputFormat** @fmt, string @url, void* @logctx, uint @offset, uint @max_probe_size) => vectors.av_probe_input_buffer2(@pb, @fmt, @url, @logctx, @offset, @max_probe_size);
    
    /// <summary>Guess the file format.</summary>
    /// <param name="pd">data to be probed</param>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    public static AVInputFormat* av_probe_input_format(AVProbeData* @pd, int @is_opened) => vectors.av_probe_input_format(@pd, @is_opened);
    
    /// <summary>Guess the file format.</summary>
    /// <param name="pd">data to be probed</param>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    /// <param name="score_max">A probe score larger that this is required to accept a detection, the variable is set to the actual detection score afterwards. If the score is &lt; = AVPROBE_SCORE_MAX / 4 it is recommended to retry with a larger probe buffer.</param>
    public static AVInputFormat* av_probe_input_format2(AVProbeData* @pd, int @is_opened, int* @score_max) => vectors.av_probe_input_format2(@pd, @is_opened, @score_max);
    
    /// <summary>Guess the file format.</summary>
    /// <param name="is_opened">Whether the file is already opened; determines whether demuxers with or without AVFMT_NOFILE are probed.</param>
    /// <param name="score_ret">The score of the best detection.</param>
    public static AVInputFormat* av_probe_input_format3(AVProbeData* @pd, int @is_opened, int* @score_ret) => vectors.av_probe_input_format3(@pd, @is_opened, @score_ret);
    
    public static void av_program_add_stream_index(AVFormatContext* @ac, int @progid, uint @idx) => vectors.av_program_add_stream_index(@ac, @progid, @idx);
    
    /// <summary>Convert an AVRational to a IEEE 32-bit `float` expressed in fixed-point format.</summary>
    /// <param name="q">Rational to be converted</param>
    /// <returns>Equivalent floating-point value, expressed as an unsigned 32-bit integer.</returns>
    public static uint av_q2intfloat(AVRational @q) => vectors.av_q2intfloat(@q);
    
    /// <summary>Return the next frame of a stream. This function returns what is stored in the file, and does not validate that what is there are valid frames for the decoder. It will split what is stored in the file into frames and return one for each call. It will not omit invalid data between valid frames so as to give the decoder the maximum information possible for decoding.</summary>
    /// <returns>0 if OK, &lt; 0 on error or end of file. On error, pkt will be blank (as if it came from av_packet_alloc()).</returns>
    public static int av_read_frame(AVFormatContext* @s, AVPacket* @pkt) => vectors.av_read_frame(@s, @pkt);
    
    public static void av_read_image_line(ushort* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component) => vectors.av_read_image_line(@dst, @data, @linesize, @desc, @x, @y, @c, @w, @read_pal_component);
    
    /// <summary>Read a line from an image, and write the values of the pixel format component c to dst.</summary>
    /// <param name="data">the array containing the pointers to the planes of the image</param>
    /// <param name="linesize">the array containing the linesizes of the image</param>
    /// <param name="desc">the pixel format descriptor for the image</param>
    /// <param name="x">the horizontal coordinate of the first pixel to read</param>
    /// <param name="y">the vertical coordinate of the first pixel to read</param>
    /// <param name="w">the width of the line to read, that is the number of values to write to dst</param>
    /// <param name="read_pal_component">if not zero and the format is a paletted format writes the values corresponding to the palette component c in data[1] to dst, rather than the palette indexes in data[0]. The behavior is undefined if the format is not paletted.</param>
    /// <param name="dst_element_size">size of elements in dst array (2 or 4 byte)</param>
    public static void av_read_image_line2(void* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component, int @dst_element_size) => vectors.av_read_image_line2(@dst, @data, @linesize, @desc, @x, @y, @c, @w, @read_pal_component, @dst_element_size);
    
    /// <summary>Pause a network-based stream (e.g. RTSP stream).</summary>
    public static int av_read_pause(AVFormatContext* @s) => vectors.av_read_pause(@s);
    
    /// <summary>Start playing a network-based stream (e.g. RTSP stream) at the current position.</summary>
    public static int av_read_play(AVFormatContext* @s) => vectors.av_read_play(@s);
    
    /// <summary>Allocate, reallocate, or free a block of memory.</summary>
    /// <param name="ptr">Pointer to a memory block already allocated with av_realloc() or `NULL`</param>
    /// <param name="size">Size in bytes of the memory block to be allocated or reallocated</param>
    /// <returns>Pointer to a newly-reallocated block or `NULL` if the block cannot be reallocated</returns>
    public static void* av_realloc(void* @ptr, ulong @size) => vectors.av_realloc(@ptr, @size);
    
    /// <summary>Allocate, reallocate, or free an array.</summary>
    /// <param name="ptr">Pointer to a memory block already allocated with av_realloc() or `NULL`</param>
    /// <param name="nmemb">Number of elements in the array</param>
    /// <param name="size">Size of the single element of the array</param>
    /// <returns>Pointer to a newly-reallocated block or NULL if the block cannot be reallocated</returns>
    public static void* av_realloc_array(void* @ptr, ulong @nmemb, ulong @size) => vectors.av_realloc_array(@ptr, @nmemb, @size);
    
    /// <summary>Allocate, reallocate, or free a block of memory.</summary>
    public static void* av_realloc_f(void* @ptr, ulong @nelem, ulong @elsize) => vectors.av_realloc_f(@ptr, @nelem, @elsize);
    
    /// <summary>Allocate, reallocate, or free a block of memory through a pointer to a pointer.</summary>
    /// <param name="ptr">Pointer to a pointer to a memory block already allocated with av_realloc(), or a pointer to `NULL`. The pointer is updated on success, or freed on failure.</param>
    /// <param name="size">Size in bytes for the memory block to be allocated or reallocated</param>
    /// <returns>Zero on success, an AVERROR error code on failure</returns>
    public static int av_reallocp(void* @ptr, ulong @size) => vectors.av_reallocp(@ptr, @size);
    
    /// <summary>Allocate, reallocate an array through a pointer to a pointer.</summary>
    /// <param name="ptr">Pointer to a pointer to a memory block already allocated with av_realloc(), or a pointer to `NULL`. The pointer is updated on success, or freed on failure.</param>
    /// <param name="nmemb">Number of elements</param>
    /// <param name="size">Size of the single element</param>
    /// <returns>Zero on success, an AVERROR error code on failure</returns>
    public static int av_reallocp_array(void* @ptr, ulong @nmemb, ulong @size) => vectors.av_reallocp_array(@ptr, @nmemb, @size);
    
    /// <summary>Reduce a fraction.</summary>
    /// <param name="dst_num">Destination numerator</param>
    /// <param name="dst_den">Destination denominator</param>
    /// <param name="num">Source numerator</param>
    /// <param name="den">Source denominator</param>
    /// <param name="max">Maximum allowed values for `dst_num` &amp; `dst_den`</param>
    /// <returns>1 if the operation is exact, 0 otherwise</returns>
    public static int av_reduce(int* @dst_num, int* @dst_den, long @num, long @den, long @max) => vectors.av_reduce(@dst_num, @dst_den, @num, @den, @max);
    
    /// <summary>Rescale a 64-bit integer with rounding to nearest.</summary>
    public static long av_rescale(long @a, long @b, long @c) => vectors.av_rescale(@a, @b, @c);
    
    /// <summary>Rescale a timestamp while preserving known durations.</summary>
    /// <param name="in_tb">Input time base</param>
    /// <param name="in_ts">Input timestamp</param>
    /// <param name="fs_tb">Duration time base; typically this is finer-grained (greater) than `in_tb` and `out_tb`</param>
    /// <param name="duration">Duration till the next call to this function (i.e. duration of the current packet/frame)</param>
    /// <param name="last">Pointer to a timestamp expressed in terms of `fs_tb`, acting as a state variable</param>
    /// <param name="out_tb">Output timebase</param>
    /// <returns>Timestamp expressed in terms of `out_tb`</returns>
    public static long av_rescale_delta(AVRational @in_tb, long @in_ts, AVRational @fs_tb, int @duration, long* @last, AVRational @out_tb) => vectors.av_rescale_delta(@in_tb, @in_ts, @fs_tb, @duration, @last, @out_tb);
    
    /// <summary>Rescale a 64-bit integer by 2 rational numbers.</summary>
    public static long av_rescale_q(long @a, AVRational @bq, AVRational @cq) => vectors.av_rescale_q(@a, @bq, @cq);
    
    /// <summary>Rescale a 64-bit integer by 2 rational numbers with specified rounding.</summary>
    public static long av_rescale_q_rnd(long @a, AVRational @bq, AVRational @cq, AVRounding @rnd) => vectors.av_rescale_q_rnd(@a, @bq, @cq, @rnd);
    
    /// <summary>Rescale a 64-bit integer with specified rounding.</summary>
    public static long av_rescale_rnd(long @a, long @b, long @c, AVRounding @rnd) => vectors.av_rescale_rnd(@a, @b, @c, @rnd);
    
    /// <summary>Check if the sample format is planar.</summary>
    /// <param name="sample_fmt">the sample format to inspect</param>
    /// <returns>1 if the sample format is planar, 0 if it is interleaved</returns>
    public static int av_sample_fmt_is_planar(AVSampleFormat @sample_fmt) => vectors.av_sample_fmt_is_planar(@sample_fmt);
    
    /// <summary>Allocate a samples buffer for nb_samples samples, and fill data pointers and linesize accordingly. The allocated samples buffer can be freed by using av_freep(&amp;audio_data[0]) Allocated data will be initialized to silence.</summary>
    /// <param name="audio_data">array to be filled with the pointer for each channel</param>
    /// <param name="linesize">aligned size for audio buffer(s), may be NULL</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="nb_samples">number of samples per channel</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>&gt;=0 on success or a negative error code on failure</returns>
    public static int av_samples_alloc(byte** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) => vectors.av_samples_alloc(@audio_data, @linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
    
    /// <summary>Allocate a data pointers array, samples buffer for nb_samples samples, and fill data pointers and linesize accordingly.</summary>
    public static int av_samples_alloc_array_and_samples(byte*** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) => vectors.av_samples_alloc_array_and_samples(@audio_data, @linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
    
    /// <summary>Copy samples from src to dst.</summary>
    /// <param name="dst">destination array of pointers to data planes</param>
    /// <param name="src">source array of pointers to data planes</param>
    /// <param name="dst_offset">offset in samples at which the data will be written to dst</param>
    /// <param name="src_offset">offset in samples at which the data will be read from src</param>
    /// <param name="nb_samples">number of samples to be copied</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="sample_fmt">audio sample format</param>
    public static int av_samples_copy(byte** @dst, byte** @src, int @dst_offset, int @src_offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt) => vectors.av_samples_copy(@dst, @src, @dst_offset, @src_offset, @nb_samples, @nb_channels, @sample_fmt);
    
    /// <summary>Fill plane data pointers and linesize for samples with sample format sample_fmt.</summary>
    /// <param name="audio_data">array to be filled with the pointer for each channel</param>
    /// <param name="linesize">calculated linesize, may be NULL</param>
    /// <param name="buf">the pointer to a buffer containing the samples</param>
    /// <param name="nb_channels">the number of channels</param>
    /// <param name="nb_samples">the number of samples in a single channel</param>
    /// <param name="sample_fmt">the sample format</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>minimum size in bytes required for the buffer on success, or a negative error code on failure</returns>
    public static int av_samples_fill_arrays(byte** @audio_data, int* @linesize, byte* @buf, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) => vectors.av_samples_fill_arrays(@audio_data, @linesize, @buf, @nb_channels, @nb_samples, @sample_fmt, @align);
    
    /// <summary>Get the required buffer size for the given audio parameters.</summary>
    /// <param name="linesize">calculated linesize, may be NULL</param>
    /// <param name="nb_channels">the number of channels</param>
    /// <param name="nb_samples">the number of samples in a single channel</param>
    /// <param name="sample_fmt">the sample format</param>
    /// <param name="align">buffer size alignment (0 = default, 1 = no alignment)</param>
    /// <returns>required buffer size, or negative error code on failure</returns>
    public static int av_samples_get_buffer_size(int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) => vectors.av_samples_get_buffer_size(@linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
    
    /// <summary>Fill an audio buffer with silence.</summary>
    /// <param name="audio_data">array of pointers to data planes</param>
    /// <param name="offset">offset in samples at which to start filling</param>
    /// <param name="nb_samples">number of samples to fill</param>
    /// <param name="nb_channels">number of audio channels</param>
    /// <param name="sample_fmt">audio sample format</param>
    public static int av_samples_set_silence(byte** @audio_data, int @offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt) => vectors.av_samples_set_silence(@audio_data, @offset, @nb_samples, @nb_channels, @sample_fmt);
    
    /// <summary>Generate an SDP for an RTP session.</summary>
    /// <param name="ac">array of AVFormatContexts describing the RTP streams. If the array is composed by only one context, such context can contain multiple AVStreams (one AVStream per RTP stream). Otherwise, all the contexts in the array (an AVCodecContext per RTP stream) must contain only one AVStream.</param>
    /// <param name="n_files">number of AVCodecContexts contained in ac</param>
    /// <param name="buf">buffer where the SDP will be stored (must be allocated by the caller)</param>
    /// <param name="size">the size of the buffer</param>
    /// <returns>0 if OK, AVERROR_xxx on error</returns>
    public static int av_sdp_create(AVFormatContext** @ac, int @n_files, byte* @buf, int @size) => vectors.av_sdp_create(@ac, @n_files, @buf, @size);
    
    /// <summary>Seek to the keyframe at timestamp. &apos;timestamp&apos; in &apos;stream_index&apos;.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream_index">If stream_index is (-1), a default stream is selected, and timestamp is automatically converted from AV_TIME_BASE units to the stream specific time_base.</param>
    /// <param name="timestamp">Timestamp in AVStream.time_base units or, if no stream is specified, in AV_TIME_BASE units.</param>
    /// <param name="flags">flags which select direction and seeking mode</param>
    /// <returns>&gt;= 0 on success</returns>
    public static int av_seek_frame(AVFormatContext* @s, int @stream_index, long @timestamp, int @flags) => vectors.av_seek_frame(@s, @stream_index, @timestamp, @flags);
    
    /// <summary>Parse the key/value pairs list in opts. For each key/value pair found, stores the value in the field in ctx that is named like the key. ctx must be an AVClass context, storing is done using AVOptions.</summary>
    /// <param name="opts">options string to parse, may be NULL</param>
    /// <param name="key_val_sep">a 0-terminated list of characters used to separate key from value</param>
    /// <param name="pairs_sep">a 0-terminated list of characters used to separate two pairs from each other</param>
    /// <returns>the number of successfully set key/value pairs, or a negative value corresponding to an AVERROR code in case of error: AVERROR(EINVAL) if opts cannot be parsed, the error code issued by av_opt_set() if a key/value pair cannot be set</returns>
    public static int av_set_options_string(void* @ctx, string @opts, string @key_val_sep, string @pairs_sep) => vectors.av_set_options_string(@ctx, @opts, @key_val_sep, @pairs_sep);
    
    /// <summary>Reduce packet size, correctly zeroing padding</summary>
    /// <param name="pkt">packet</param>
    /// <param name="size">new size</param>
    public static void av_shrink_packet(AVPacket* @pkt, int @size) => vectors.av_shrink_packet(@pkt, @size);
    
    /// <summary>Multiply two `size_t` values checking for overflow.</summary>
    /// <param name="r">Pointer to the result of the operation</param>
    /// <returns>0 on success, AVERROR(EINVAL) on overflow</returns>
    public static int av_size_mult(ulong @a, ulong @b, ulong* @r) => vectors.av_size_mult(@a, @b, @r);
    
    /// <summary>Duplicate a string.</summary>
    /// <param name="s">String to be duplicated</param>
    /// <returns>Pointer to a newly-allocated string containing a copy of `s` or `NULL` if the string cannot be allocated</returns>
    public static byte* av_strdup(string @s) => vectors.av_strdup(@s);
    
    /// <summary>Wrap an existing array as stream side data.</summary>
    /// <param name="st">stream</param>
    /// <param name="type">side information type</param>
    /// <param name="data">the side data array. It must be allocated with the av_malloc() family of functions. The ownership of the data is transferred to st.</param>
    /// <param name="size">side information size</param>
    /// <returns>zero on success, a negative AVERROR code on failure. On failure, the stream is unchanged and the data remains owned by the caller.</returns>
    public static int av_stream_add_side_data(AVStream* @st, AVPacketSideDataType @type, byte* @data, ulong @size) => vectors.av_stream_add_side_data(@st, @type, @data, @size);
    
    /// <summary>Get the AVClass for AVStream. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* av_stream_get_class() => vectors.av_stream_get_class();
    
    /// <summary>Get the internal codec timebase from a stream.</summary>
    /// <param name="st">input stream to extract the timebase from</param>
    public static AVRational av_stream_get_codec_timebase(AVStream* @st) => vectors.av_stream_get_codec_timebase(@st);
    
    /// <summary>Returns the pts of the last muxed packet + its duration</summary>
    public static long av_stream_get_end_pts(AVStream* @st) => vectors.av_stream_get_end_pts(@st);
    
    public static AVCodecParserContext* av_stream_get_parser(AVStream* @s) => vectors.av_stream_get_parser(@s);
    
    /// <summary>Get side information from stream.</summary>
    /// <param name="stream">stream</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">If supplied, *size will be set to the size of the side data or to zero if the desired side data is not present.</param>
    /// <returns>pointer to data if present or NULL otherwise</returns>
    public static byte* av_stream_get_side_data(AVStream* @stream, AVPacketSideDataType @type, ulong* @size) => vectors.av_stream_get_side_data(@stream, @type, @size);
    
    /// <summary>Allocate new information from stream.</summary>
    /// <param name="stream">stream</param>
    /// <param name="type">desired side information type</param>
    /// <param name="size">side information size</param>
    /// <returns>pointer to fresh allocated data or NULL otherwise</returns>
    public static byte* av_stream_new_side_data(AVStream* @stream, AVPacketSideDataType @type, ulong @size) => vectors.av_stream_new_side_data(@stream, @type, @size);
    
    /// <summary>Put a description of the AVERROR code errnum in errbuf. In case of failure the global variable errno is set to indicate the error. Even in case of failure av_strerror() will print a generic error message indicating the errnum provided to errbuf.</summary>
    /// <param name="errnum">error code to describe</param>
    /// <param name="errbuf">buffer to which description is written</param>
    /// <param name="errbuf_size">the size in bytes of errbuf</param>
    /// <returns>0 on success, a negative value if a description for errnum cannot be found</returns>
    public static int av_strerror(int @errnum, byte* @errbuf, ulong @errbuf_size) => vectors.av_strerror(@errnum, @errbuf, @errbuf_size);
    
    /// <summary>Duplicate a substring of a string.</summary>
    /// <param name="s">String to be duplicated</param>
    /// <param name="len">Maximum length of the resulting string (not counting the terminating byte)</param>
    /// <returns>Pointer to a newly-allocated string containing a substring of `s` or `NULL` if the string cannot be allocated</returns>
    public static byte* av_strndup(string @s, ulong @len) => vectors.av_strndup(@s, @len);
    
    /// <summary>Subtract one rational from another.</summary>
    /// <param name="b">First rational</param>
    /// <param name="c">Second rational</param>
    /// <returns>b-c</returns>
    public static AVRational av_sub_q(AVRational @b, AVRational @c) => vectors.av_sub_q(@b, @c);
    
    /// <summary>Wrapper to work around the lack of mkstemp() on mingw. Also, tries to create file in /tmp first, if possible. *prefix can be a character constant; *filename will be allocated internally.</summary>
    /// <returns>file descriptor of opened file (or negative value corresponding to an AVERROR code on error) and opened file name in **filename.</returns>
    [Obsolete("as fd numbers cannot be passed saftely between libs on some platforms")]
    public static int av_tempfile(string @prefix, byte** @filename, int @log_offset, void* @log_ctx) => vectors.av_tempfile(@prefix, @filename, @log_offset, @log_ctx);
    
    /// <summary>Adjust frame number for NTSC drop frame time code.</summary>
    /// <param name="framenum">frame number to adjust</param>
    /// <param name="fps">frame per second, multiples of 30</param>
    /// <returns>adjusted frame number</returns>
    public static int av_timecode_adjust_ntsc_framenum2(int @framenum, int @fps) => vectors.av_timecode_adjust_ntsc_framenum2(@framenum, @fps);
    
    /// <summary>Check if the timecode feature is available for the given frame rate</summary>
    /// <returns>0 if supported, &lt; 0 otherwise</returns>
    public static int av_timecode_check_frame_rate(AVRational @rate) => vectors.av_timecode_check_frame_rate(@rate);
    
    /// <summary>Convert sei info to SMPTE 12M binary representation.</summary>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="drop">drop flag</param>
    /// <param name="hh">hour</param>
    /// <param name="mm">minute</param>
    /// <param name="ss">second</param>
    /// <param name="ff">frame number</param>
    /// <returns>the SMPTE binary representation</returns>
    public static uint av_timecode_get_smpte(AVRational @rate, int @drop, int @hh, int @mm, int @ss, int @ff) => vectors.av_timecode_get_smpte(@rate, @drop, @hh, @mm, @ss, @ff);
    
    /// <summary>Convert frame number to SMPTE 12M binary representation.</summary>
    /// <param name="tc">timecode data correctly initialized</param>
    /// <param name="framenum">frame number</param>
    /// <returns>the SMPTE binary representation</returns>
    public static uint av_timecode_get_smpte_from_framenum(AVTimecode* @tc, int @framenum) => vectors.av_timecode_get_smpte_from_framenum(@tc, @framenum);
    
    /// <summary>Init a timecode struct with the passed parameters.</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="flags">miscellaneous flags such as drop frame, +24 hours, ... (see AVTimecodeFlag)</param>
    /// <param name="frame_start">the first frame number</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log)</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    public static int av_timecode_init(AVTimecode* @tc, AVRational @rate, int @flags, int @frame_start, void* @log_ctx) => vectors.av_timecode_init(@tc, @rate, @flags, @frame_start, @log_ctx);
    
    /// <summary>Init a timecode struct from the passed timecode components.</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="flags">miscellaneous flags such as drop frame, +24 hours, ... (see AVTimecodeFlag)</param>
    /// <param name="hh">hours</param>
    /// <param name="mm">minutes</param>
    /// <param name="ss">seconds</param>
    /// <param name="ff">frames</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log)</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    public static int av_timecode_init_from_components(AVTimecode* @tc, AVRational @rate, int @flags, int @hh, int @mm, int @ss, int @ff, void* @log_ctx) => vectors.av_timecode_init_from_components(@tc, @rate, @flags, @hh, @mm, @ss, @ff, @log_ctx);
    
    /// <summary>Parse timecode representation (hh:mm:ss[:;.]ff).</summary>
    /// <param name="tc">pointer to an allocated AVTimecode</param>
    /// <param name="rate">frame rate in rational form</param>
    /// <param name="str">timecode string which will determine the frame start</param>
    /// <param name="log_ctx">a pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct (used for av_log).</param>
    /// <returns>0 on success, AVERROR otherwise</returns>
    public static int av_timecode_init_from_string(AVTimecode* @tc, AVRational @rate, string @str, void* @log_ctx) => vectors.av_timecode_init_from_string(@tc, @rate, @str, @log_ctx);
    
    /// <summary>Get the timecode string from the 25-bit timecode format (MPEG GOP format).</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="tc25bit">the 25-bits timecode</param>
    /// <returns>the buf parameter</returns>
    public static byte* av_timecode_make_mpeg_tc_string(byte* @buf, uint @tc25bit) => vectors.av_timecode_make_mpeg_tc_string(@buf, @tc25bit);
    
    /// <summary>Get the timecode string from the SMPTE timecode format.</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="tcsmpte">the 32-bit SMPTE timecode</param>
    /// <param name="prevent_df">prevent the use of a drop flag when it is known the DF bit is arbitrary</param>
    /// <returns>the buf parameter</returns>
    public static byte* av_timecode_make_smpte_tc_string(byte* @buf, uint @tcsmpte, int @prevent_df) => vectors.av_timecode_make_smpte_tc_string(@buf, @tcsmpte, @prevent_df);
    
    /// <summary>Get the timecode string from the SMPTE timecode format.</summary>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="rate">frame rate of the timecode</param>
    /// <param name="tcsmpte">the 32-bit SMPTE timecode</param>
    /// <param name="prevent_df">prevent the use of a drop flag when it is known the DF bit is arbitrary</param>
    /// <param name="skip_field">prevent the use of a field flag when it is known the field bit is arbitrary (e.g. because it is used as PC flag)</param>
    /// <returns>the buf parameter</returns>
    public static byte* av_timecode_make_smpte_tc_string2(byte* @buf, AVRational @rate, uint @tcsmpte, int @prevent_df, int @skip_field) => vectors.av_timecode_make_smpte_tc_string2(@buf, @rate, @tcsmpte, @prevent_df, @skip_field);
    
    /// <summary>Load timecode string in buf.</summary>
    /// <param name="tc">timecode data correctly initialized</param>
    /// <param name="buf">destination buffer, must be at least AV_TIMECODE_STR_SIZE long</param>
    /// <param name="framenum">frame number</param>
    /// <returns>the buf parameter</returns>
    public static byte* av_timecode_make_string(AVTimecode* @tc, byte* @buf, int @framenum) => vectors.av_timecode_make_string(@tc, @buf, @framenum);
    
    public static void av_tree_destroy(AVTreeNode* @t) => vectors.av_tree_destroy(@t);
    
    /// <summary>Apply enu(opaque, &amp;elem) to all the elements in the tree in a given range.</summary>
    /// <param name="cmp">a comparison function that returns &lt; 0 for an element below the range, &gt; 0 for an element above the range and == 0 for an element inside the range</param>
    public static void av_tree_enumerate(AVTreeNode* @t, void* @opaque, av_tree_enumerate_cmp_func @cmp, av_tree_enumerate_enu_func @enu) => vectors.av_tree_enumerate(@t, @opaque, @cmp, @enu);
    
    /// <summary>Find an element.</summary>
    /// <param name="root">a pointer to the root node of the tree</param>
    /// <param name="cmp">compare function used to compare elements in the tree, API identical to that of Standard C&apos;s qsort It is guaranteed that the first and only the first argument to cmp() will be the key parameter to av_tree_find(), thus it could if the user wants, be a different type (like an opaque context).</param>
    /// <param name="next">If next is not NULL, then next[0] will contain the previous element and next[1] the next element. If either does not exist, then the corresponding entry in next is unchanged.</param>
    /// <returns>An element with cmp(key, elem) == 0 or NULL if no such element exists in the tree.</returns>
    public static void* av_tree_find(AVTreeNode* @root, void* @key, av_tree_find_cmp_func @cmp, ref void_ptrArray2 @next) => vectors.av_tree_find(@root, @key, @cmp, ref @next);
    
    /// <summary>Insert or remove an element.</summary>
    /// <param name="rootp">A pointer to a pointer to the root node of the tree; note that the root node can change during insertions, this is required to keep the tree balanced.</param>
    /// <param name="key">pointer to the element key to insert in the tree</param>
    /// <param name="cmp">compare function used to compare elements in the tree, API identical to that of Standard C&apos;s qsort</param>
    /// <param name="next">Used to allocate and free AVTreeNodes. For insertion the user must set it to an allocated and zeroed object of at least av_tree_node_size bytes size. av_tree_insert() will set it to NULL if it has been consumed. For deleting elements *next is set to NULL by the user and av_tree_insert() will set it to the AVTreeNode which was used for the removed element. This allows the use of flat arrays, which have lower overhead compared to many malloced elements. You might want to define a function like:</param>
    /// <returns>If no insertion happened, the found element; if an insertion or removal happened, then either key or NULL will be returned. Which one it is depends on the tree state and the implementation. You should make no assumptions that it&apos;s one or the other in the code.</returns>
    public static void* av_tree_insert(AVTreeNode** @rootp, void* @key, av_tree_insert_cmp_func @cmp, AVTreeNode** @next) => vectors.av_tree_insert(@rootp, @key, @cmp, @next);
    
    /// <summary>Allocate an AVTreeNode.</summary>
    public static AVTreeNode* av_tree_node_alloc() => vectors.av_tree_node_alloc();
    
    /// <summary>Split a URL string into components.</summary>
    /// <param name="proto">the buffer for the protocol</param>
    /// <param name="proto_size">the size of the proto buffer</param>
    /// <param name="authorization">the buffer for the authorization</param>
    /// <param name="authorization_size">the size of the authorization buffer</param>
    /// <param name="hostname">the buffer for the host name</param>
    /// <param name="hostname_size">the size of the hostname buffer</param>
    /// <param name="port_ptr">a pointer to store the port number in</param>
    /// <param name="path">the buffer for the path</param>
    /// <param name="path_size">the size of the path buffer</param>
    /// <param name="url">the URL to split</param>
    public static void av_url_split(byte* @proto, int @proto_size, byte* @authorization, int @authorization_size, byte* @hostname, int @hostname_size, int* @port_ptr, byte* @path, int @path_size, string @url) => vectors.av_url_split(@proto, @proto_size, @authorization, @authorization_size, @hostname, @hostname_size, @port_ptr, @path, @path_size, @url);
    
    /// <summary>Sleep for a period of time. Although the duration is expressed in microseconds, the actual delay may be rounded to the precision of the system timer.</summary>
    /// <param name="usec">Number of microseconds to sleep.</param>
    /// <returns>zero on success or (negative) error code.</returns>
    public static int av_usleep(uint @usec) => vectors.av_usleep(@usec);
    
    /// <summary>Return an informative version string. This usually is the actual release version number or a git commit description. This string has no fixed format and can change any time. It should never be parsed by code.</summary>
    public static string av_version_info() => vectors.av_version_info();
    
    /// <summary>Send the specified message to the log if the level is less than or equal to the current av_log_level. By default, all logging messages are sent to stderr. This behavior can be altered by setting a different logging callback function.</summary>
    /// <param name="avcl">A pointer to an arbitrary struct of which the first field is a pointer to an AVClass struct.</param>
    /// <param name="level">The importance level of the message expressed using a &quot;Logging Constant&quot;.</param>
    /// <param name="fmt">The format string (printf-compatible) that specifies how subsequent arguments are converted to output.</param>
    /// <param name="vl">The arguments referenced by the format string.</param>
    public static void av_vlog(void* @avcl, int @level, string @fmt, byte* @vl) => vectors.av_vlog(@avcl, @level, @fmt, @vl);
    
    /// <summary>Write a packet to an output media file.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="pkt">The packet containing the data to be written. Note that unlike av_interleaved_write_frame(), this function does not take ownership of the packet passed to it (though some muxers may make an internal reference to the input packet).  This parameter can be NULL (at any time, not just at the end), in order to immediately flush data buffered within the muxer, for muxers that buffer up data internally before writing it to the output.  Packet&apos;s &quot;stream_index&quot; field must be set to the index of the corresponding stream in &quot;s-&gt;streams&quot;.  The timestamps ( &quot;pts&quot;, &quot;dts&quot;) must be set to correct values in the stream&apos;s timebase (unless the output format is flagged with the AVFMT_NOTIMESTAMPS flag, then they can be set to AV_NOPTS_VALUE). The dts for subsequent packets passed to this function must be strictly increasing when compared in their respective timebases (unless the output format is flagged with the AVFMT_TS_NONSTRICT, then they merely have to be nondecreasing). &quot;duration&quot;) should also be set if known.</param>
    /// <returns>&lt; 0 on error, = 0 if OK, 1 if flushed and there is no more data to flush</returns>
    public static int av_write_frame(AVFormatContext* @s, AVPacket* @pkt) => vectors.av_write_frame(@s, @pkt);
    
    public static void av_write_image_line(ushort* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w) => vectors.av_write_image_line(@src, ref @data, @linesize, @desc, @x, @y, @c, @w);
    
    /// <summary>Write the values from src to the pixel format component c of an image line.</summary>
    /// <param name="src">array containing the values to write</param>
    /// <param name="data">the array containing the pointers to the planes of the image to write into. It is supposed to be zeroed.</param>
    /// <param name="linesize">the array containing the linesizes of the image</param>
    /// <param name="desc">the pixel format descriptor for the image</param>
    /// <param name="x">the horizontal coordinate of the first pixel to write</param>
    /// <param name="y">the vertical coordinate of the first pixel to write</param>
    /// <param name="w">the width of the line to write, that is the number of values to write to the image line</param>
    /// <param name="src_element_size">size of elements in src array (2 or 4 byte)</param>
    public static void av_write_image_line2(void* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @src_element_size) => vectors.av_write_image_line2(@src, ref @data, @linesize, @desc, @x, @y, @c, @w, @src_element_size);
    
    /// <summary>Write the stream trailer to an output media file and free the file private data.</summary>
    /// <param name="s">media file handle</param>
    /// <returns>0 if OK, AVERROR_xxx on error</returns>
    public static int av_write_trailer(AVFormatContext* @s) => vectors.av_write_trailer(@s);
    
    /// <summary>Write an uncoded frame to an output media file.</summary>
    public static int av_write_uncoded_frame(AVFormatContext* @s, int @stream_index, AVFrame* @frame) => vectors.av_write_uncoded_frame(@s, @stream_index, @frame);
    
    /// <summary>Test whether a muxer supports uncoded frame.</summary>
    /// <returns>&gt;=0 if an uncoded frame can be written to that muxer and stream,  &lt; 0 if not</returns>
    public static int av_write_uncoded_frame_query(AVFormatContext* @s, int @stream_index) => vectors.av_write_uncoded_frame_query(@s, @stream_index);
    
    /// <summary>Encode extradata length to a buffer. Used by xiph codecs.</summary>
    /// <param name="s">buffer to write to; must be at least (v/255+1) bytes long</param>
    /// <param name="v">size of extradata in bytes</param>
    /// <returns>number of bytes written to the buffer.</returns>
    public static uint av_xiphlacing(byte* @s, uint @v) => vectors.av_xiphlacing(@s, @v);
    
    /// <summary>Modify width and height values so that they will result in a memory buffer that is acceptable for the codec if you do not use any horizontal padding.</summary>
    public static void avcodec_align_dimensions(AVCodecContext* @s, int* @width, int* @height) => vectors.avcodec_align_dimensions(@s, @width, @height);
    
    /// <summary>Modify width and height values so that they will result in a memory buffer that is acceptable for the codec if you also ensure that all line sizes are a multiple of the respective linesize_align[i].</summary>
    public static void avcodec_align_dimensions2(AVCodecContext* @s, int* @width, int* @height, ref int_array8 @linesize_align) => vectors.avcodec_align_dimensions2(@s, @width, @height, ref @linesize_align);
    
    /// <summary>Allocate an AVCodecContext and set its fields to default values. The resulting struct should be freed with avcodec_free_context().</summary>
    /// <param name="codec">if non-NULL, allocate private data and initialize defaults for the given codec. It is illegal to then call avcodec_open2() with a different codec. If NULL, then the codec-specific defaults won&apos;t be initialized, which may result in suboptimal default settings (this is important mainly for encoders, e.g. libx264).</param>
    /// <returns>An AVCodecContext filled with default values or NULL on failure.</returns>
    public static AVCodecContext* avcodec_alloc_context3(AVCodec* @codec) => vectors.avcodec_alloc_context3(@codec);
    
    /// <summary>Converts swscale x/y chroma position to AVChromaLocation.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    public static AVChromaLocation avcodec_chroma_pos_to_enum(int @xpos, int @ypos) => vectors.avcodec_chroma_pos_to_enum(@xpos, @ypos);
    
    /// <summary>Close a given AVCodecContext and free all the data associated with it (but not the AVCodecContext itself).</summary>
    public static int avcodec_close(AVCodecContext* @avctx) => vectors.avcodec_close(@avctx);
    
    /// <summary>Return the libavcodec build-time configuration.</summary>
    public static string avcodec_configuration() => vectors.avcodec_configuration();
    
    /// <summary>Decode a subtitle message. Return a negative value on error, otherwise return the number of bytes used. If no subtitle could be decompressed, got_sub_ptr is zero. Otherwise, the subtitle is stored in *sub. Note that AV_CODEC_CAP_DR1 is not available for subtitle codecs. This is for simplicity, because the performance difference is expected to be negligible and reusing a get_buffer written for video codecs would probably perform badly due to a potentially very different allocation pattern.</summary>
    /// <param name="avctx">the codec context</param>
    /// <param name="sub">The preallocated AVSubtitle in which the decoded subtitle will be stored, must be freed with avsubtitle_free if *got_sub_ptr is set.</param>
    /// <param name="got_sub_ptr">Zero if no subtitle could be decompressed, otherwise, it is nonzero.</param>
    /// <param name="avpkt">The input AVPacket containing the input buffer.</param>
    public static int avcodec_decode_subtitle2(AVCodecContext* @avctx, AVSubtitle* @sub, int* @got_sub_ptr, AVPacket* @avpkt) => vectors.avcodec_decode_subtitle2(@avctx, @sub, @got_sub_ptr, @avpkt);
    
    public static int avcodec_default_execute(AVCodecContext* @c, avcodec_default_execute_func_func @func, void* @arg, int* @ret, int @count, int @size) => vectors.avcodec_default_execute(@c, @func, @arg, @ret, @count, @size);
    
    public static int avcodec_default_execute2(AVCodecContext* @c, avcodec_default_execute2_func_func @func, void* @arg, int* @ret, int @count) => vectors.avcodec_default_execute2(@c, @func, @arg, @ret, @count);
    
    /// <summary>The default callback for AVCodecContext.get_buffer2(). It is made public so it can be called by custom get_buffer2() implementations for decoders without AV_CODEC_CAP_DR1 set.</summary>
    public static int avcodec_default_get_buffer2(AVCodecContext* @s, AVFrame* @frame, int @flags) => vectors.avcodec_default_get_buffer2(@s, @frame, @flags);
    
    /// <summary>The default callback for AVCodecContext.get_encode_buffer(). It is made public so it can be called by custom get_encode_buffer() implementations for encoders without AV_CODEC_CAP_DR1 set.</summary>
    public static int avcodec_default_get_encode_buffer(AVCodecContext* @s, AVPacket* @pkt, int @flags) => vectors.avcodec_default_get_encode_buffer(@s, @pkt, @flags);
    
    public static AVPixelFormat avcodec_default_get_format(AVCodecContext* @s, AVPixelFormat* @fmt) => vectors.avcodec_default_get_format(@s, @fmt);
    
    /// <summary>Returns descriptor for given codec ID or NULL if no descriptor exists.</summary>
    /// <returns>descriptor for given codec ID or NULL if no descriptor exists.</returns>
    public static AVCodecDescriptor* avcodec_descriptor_get(AVCodecID @id) => vectors.avcodec_descriptor_get(@id);
    
    /// <summary>Returns codec descriptor with the given name or NULL if no such descriptor exists.</summary>
    /// <returns>codec descriptor with the given name or NULL if no such descriptor exists.</returns>
    public static AVCodecDescriptor* avcodec_descriptor_get_by_name(string @name) => vectors.avcodec_descriptor_get_by_name(@name);
    
    /// <summary>Iterate over all codec descriptors known to libavcodec.</summary>
    /// <param name="prev">previous descriptor. NULL to get the first descriptor.</param>
    /// <returns>next descriptor or NULL after the last descriptor</returns>
    public static AVCodecDescriptor* avcodec_descriptor_next(AVCodecDescriptor* @prev) => vectors.avcodec_descriptor_next(@prev);
    
    /// <summary>@{</summary>
    public static int avcodec_encode_subtitle(AVCodecContext* @avctx, byte* @buf, int @buf_size, AVSubtitle* @sub) => vectors.avcodec_encode_subtitle(@avctx, @buf, @buf_size, @sub);
    
    /// <summary>Converts AVChromaLocation to swscale x/y chroma position.</summary>
    /// <param name="xpos">horizontal chroma sample position</param>
    /// <param name="ypos">vertical   chroma sample position</param>
    public static int avcodec_enum_to_chroma_pos(int* @xpos, int* @ypos, AVChromaLocation @pos) => vectors.avcodec_enum_to_chroma_pos(@xpos, @ypos, @pos);
    
    /// <summary>Fill AVFrame audio data and linesize pointers.</summary>
    /// <param name="frame">the AVFrame frame-&gt;nb_samples must be set prior to calling the function. This function fills in frame-&gt;data, frame-&gt;extended_data, frame-&gt;linesize[0].</param>
    /// <param name="nb_channels">channel count</param>
    /// <param name="sample_fmt">sample format</param>
    /// <param name="buf">buffer to use for frame data</param>
    /// <param name="buf_size">size of buffer</param>
    /// <param name="align">plane size sample alignment (0 = default)</param>
    /// <returns>&gt;=0 on success, negative error code on failure</returns>
    public static int avcodec_fill_audio_frame(AVFrame* @frame, int @nb_channels, AVSampleFormat @sample_fmt, byte* @buf, int @buf_size, int @align) => vectors.avcodec_fill_audio_frame(@frame, @nb_channels, @sample_fmt, @buf, @buf_size, @align);
    
    /// <summary>Find the best pixel format to convert to given a certain source pixel format. When converting from one pixel format to another, information loss may occur. For example, when converting from RGB24 to GRAY, the color information will be lost. Similarly, other losses occur when converting from some formats to other formats. avcodec_find_best_pix_fmt_of_2() searches which of the given pixel formats should be used to suffer the least amount of loss. The pixel formats from which it chooses one, are determined by the pix_fmt_list parameter.</summary>
    /// <param name="pix_fmt_list">AV_PIX_FMT_NONE terminated array of pixel formats to choose from</param>
    /// <param name="src_pix_fmt">source pixel format</param>
    /// <param name="has_alpha">Whether the source pixel format alpha channel is used.</param>
    /// <param name="loss_ptr">Combination of flags informing you what kind of losses will occur.</param>
    /// <returns>The best pixel format to convert to or -1 if none was found.</returns>
    public static AVPixelFormat avcodec_find_best_pix_fmt_of_list(AVPixelFormat* @pix_fmt_list, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr) => vectors.avcodec_find_best_pix_fmt_of_list(@pix_fmt_list, @src_pix_fmt, @has_alpha, @loss_ptr);
    
    /// <summary>Find a registered decoder with a matching codec ID.</summary>
    /// <param name="id">AVCodecID of the requested decoder</param>
    /// <returns>A decoder if one was found, NULL otherwise.</returns>
    public static AVCodec* avcodec_find_decoder(AVCodecID @id) => vectors.avcodec_find_decoder(@id);
    
    /// <summary>Find a registered decoder with the specified name.</summary>
    /// <param name="name">name of the requested decoder</param>
    /// <returns>A decoder if one was found, NULL otherwise.</returns>
    public static AVCodec* avcodec_find_decoder_by_name(string @name) => vectors.avcodec_find_decoder_by_name(@name);
    
    /// <summary>Find a registered encoder with a matching codec ID.</summary>
    /// <param name="id">AVCodecID of the requested encoder</param>
    /// <returns>An encoder if one was found, NULL otherwise.</returns>
    public static AVCodec* avcodec_find_encoder(AVCodecID @id) => vectors.avcodec_find_encoder(@id);
    
    /// <summary>Find a registered encoder with the specified name.</summary>
    /// <param name="name">name of the requested encoder</param>
    /// <returns>An encoder if one was found, NULL otherwise.</returns>
    public static AVCodec* avcodec_find_encoder_by_name(string @name) => vectors.avcodec_find_encoder_by_name(@name);
    
    /// <summary>Reset the internal codec state / flush internal buffers. Should be called e.g. when seeking or when switching to a different stream.</summary>
    public static void avcodec_flush_buffers(AVCodecContext* @avctx) => vectors.avcodec_flush_buffers(@avctx);
    
    /// <summary>Free the codec context and everything associated with it and write NULL to the provided pointer.</summary>
    public static void avcodec_free_context(AVCodecContext** @avctx) => vectors.avcodec_free_context(@avctx);
    
    /// <summary>Get the AVClass for AVCodecContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* avcodec_get_class() => vectors.avcodec_get_class();
    
    [Obsolete("This function should not be used.")]
    public static AVClass* avcodec_get_frame_class() => vectors.avcodec_get_frame_class();
    
    /// <summary>Retrieve supported hardware configurations for a codec.</summary>
    public static AVCodecHWConfig* avcodec_get_hw_config(AVCodec* @codec, int @index) => vectors.avcodec_get_hw_config(@codec, @index);
    
    /// <summary>Create and return a AVHWFramesContext with values adequate for hardware decoding. This is meant to get called from the get_format callback, and is a helper for preparing a AVHWFramesContext for AVCodecContext.hw_frames_ctx. This API is for decoding with certain hardware acceleration modes/APIs only.</summary>
    /// <param name="avctx">The context which is currently calling get_format, and which implicitly contains all state needed for filling the returned AVHWFramesContext properly.</param>
    /// <param name="device_ref">A reference to the AVHWDeviceContext describing the device which will be used by the hardware decoder.</param>
    /// <param name="hw_pix_fmt">The hwaccel format you are going to return from get_format.</param>
    /// <param name="out_frames_ref">On success, set to a reference to an _uninitialized_ AVHWFramesContext, created from the given device_ref. Fields will be set to values required for decoding. Not changed if an error is returned.</param>
    /// <returns>zero on success, a negative value on error. The following error codes have special semantics: AVERROR(ENOENT): the decoder does not support this functionality. Setup is always manual, or it is a decoder which does not support setting AVCodecContext.hw_frames_ctx at all, or it is a software format. AVERROR(EINVAL): it is known that hardware decoding is not supported for this configuration, or the device_ref is not supported for the hwaccel referenced by hw_pix_fmt.</returns>
    public static int avcodec_get_hw_frames_parameters(AVCodecContext* @avctx, AVBufferRef* @device_ref, AVPixelFormat @hw_pix_fmt, AVBufferRef** @out_frames_ref) => vectors.avcodec_get_hw_frames_parameters(@avctx, @device_ref, @hw_pix_fmt, @out_frames_ref);
    
    /// <summary>Get the name of a codec.</summary>
    /// <returns>a static string identifying the codec; never NULL</returns>
    public static string avcodec_get_name(AVCodecID @id) => vectors.avcodec_get_name(@id);
    
    /// <summary>Get the AVClass for AVSubtitleRect. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* avcodec_get_subtitle_rect_class() => vectors.avcodec_get_subtitle_rect_class();
    
    /// <summary>Get the type of the given codec.</summary>
    public static AVMediaType avcodec_get_type(AVCodecID @codec_id) => vectors.avcodec_get_type(@codec_id);
    
    /// <summary>Returns a positive value if s is open (i.e. avcodec_open2() was called on it with no corresponding avcodec_close()), 0 otherwise.</summary>
    /// <returns>a positive value if s is open (i.e. avcodec_open2() was called on it with no corresponding avcodec_close()), 0 otherwise.</returns>
    public static int avcodec_is_open(AVCodecContext* @s) => vectors.avcodec_is_open(@s);
    
    /// <summary>Return the libavcodec license.</summary>
    public static string avcodec_license() => vectors.avcodec_license();
    
    /// <summary>Initialize the AVCodecContext to use the given AVCodec. Prior to using this function the context has to be allocated with avcodec_alloc_context3().</summary>
    /// <param name="avctx">The context to initialize.</param>
    /// <param name="codec">The codec to open this context for. If a non-NULL codec has been previously passed to avcodec_alloc_context3() or for this context, then this parameter MUST be either NULL or equal to the previously passed codec.</param>
    /// <param name="options">A dictionary filled with AVCodecContext and codec-private options. On return this object will be filled with options that were not found.</param>
    /// <returns>zero on success, a negative value on error</returns>
    public static int avcodec_open2(AVCodecContext* @avctx, AVCodec* @codec, AVDictionary** @options) => vectors.avcodec_open2(@avctx, @codec, @options);
    
    /// <summary>Allocate a new AVCodecParameters and set its fields to default values (unknown/invalid/0). The returned struct must be freed with avcodec_parameters_free().</summary>
    public static AVCodecParameters* avcodec_parameters_alloc() => vectors.avcodec_parameters_alloc();
    
    /// <summary>Copy the contents of src to dst. Any allocated fields in dst are freed and replaced with newly allocated duplicates of the corresponding fields in src.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure.</returns>
    public static int avcodec_parameters_copy(AVCodecParameters* @dst, AVCodecParameters* @src) => vectors.avcodec_parameters_copy(@dst, @src);
    
    /// <summary>Free an AVCodecParameters instance and everything associated with it and write NULL to the supplied pointer.</summary>
    public static void avcodec_parameters_free(AVCodecParameters** @par) => vectors.avcodec_parameters_free(@par);
    
    /// <summary>Fill the parameters struct based on the values from the supplied codec context. Any allocated fields in par are freed and replaced with duplicates of the corresponding fields in codec.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure</returns>
    public static int avcodec_parameters_from_context(AVCodecParameters* @par, AVCodecContext* @codec) => vectors.avcodec_parameters_from_context(@par, @codec);
    
    /// <summary>Fill the codec context based on the values from the supplied codec parameters. Any allocated fields in codec that have a corresponding field in par are freed and replaced with duplicates of the corresponding field in par. Fields in codec that do not have a counterpart in par are not touched.</summary>
    /// <returns>&gt;= 0 on success, a negative AVERROR code on failure.</returns>
    public static int avcodec_parameters_to_context(AVCodecContext* @codec, AVCodecParameters* @par) => vectors.avcodec_parameters_to_context(@codec, @par);
    
    /// <summary>Return a value representing the fourCC code associated to the pixel format pix_fmt, or 0 if no associated fourCC code can be found.</summary>
    public static uint avcodec_pix_fmt_to_codec_tag(AVPixelFormat @pix_fmt) => vectors.avcodec_pix_fmt_to_codec_tag(@pix_fmt);
    
    /// <summary>Return a name for the specified profile, if available.</summary>
    /// <param name="codec_id">the ID of the codec to which the requested profile belongs</param>
    /// <param name="profile">the profile value for which a name is requested</param>
    /// <returns>A name for the profile if found, NULL otherwise.</returns>
    public static string avcodec_profile_name(AVCodecID @codec_id, int @profile) => vectors.avcodec_profile_name(@codec_id, @profile);
    
    /// <summary>Return decoded output data from a decoder.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame">This will be set to a reference-counted video or audio frame (depending on the decoder type) allocated by the decoder. Note that the function will always call av_frame_unref(frame) before doing anything else.</param>
    /// <returns>0:                 success, a frame was returned AVERROR(EAGAIN):   output is not available in this state - user must try to send new input AVERROR_EOF:       the decoder has been fully flushed, and there will be no more output frames AVERROR(EINVAL):   codec not opened, or it is an encoder AVERROR_INPUT_CHANGED:   current decoded frame has changed parameters with respect to first decoded frame. Applicable when flag AV_CODEC_FLAG_DROPCHANGED is set. other negative values: legitimate decoding errors</returns>
    public static int avcodec_receive_frame(AVCodecContext* @avctx, AVFrame* @frame) => vectors.avcodec_receive_frame(@avctx, @frame);
    
    /// <summary>Read encoded data from the encoder.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="avpkt">This will be set to a reference-counted packet allocated by the encoder. Note that the function will always call av_packet_unref(avpkt) before doing anything else.</param>
    /// <returns>0 on success, otherwise negative error code: AVERROR(EAGAIN):   output is not available in the current state - user must try to send input AVERROR_EOF:       the encoder has been fully flushed, and there will be no more output packets AVERROR(EINVAL):   codec not opened, or it is a decoder other errors: legitimate encoding errors</returns>
    public static int avcodec_receive_packet(AVCodecContext* @avctx, AVPacket* @avpkt) => vectors.avcodec_receive_packet(@avctx, @avpkt);
    
    /// <summary>Supply a raw video or audio frame to the encoder. Use avcodec_receive_packet() to retrieve buffered output packets.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="frame">AVFrame containing the raw audio or video frame to be encoded. Ownership of the frame remains with the caller, and the encoder will not write to the frame. The encoder may create a reference to the frame data (or copy it if the frame is not reference-counted). It can be NULL, in which case it is considered a flush packet.  This signals the end of the stream. If the encoder still has packets buffered, it will return them after this call. Once flushing mode has been entered, additional flush packets are ignored, and sending frames will return AVERROR_EOF.</param>
    /// <returns>0 on success, otherwise negative error code: AVERROR(EAGAIN):   input is not accepted in the current state - user must read output with avcodec_receive_packet() (once all output is read, the packet should be resent, and the call will not fail with EAGAIN). AVERROR_EOF:       the encoder has been flushed, and no new frames can be sent to it AVERROR(EINVAL):   codec not opened, it is a decoder, or requires flush AVERROR(ENOMEM):   failed to add packet to internal queue, or similar other errors: legitimate encoding errors</returns>
    public static int avcodec_send_frame(AVCodecContext* @avctx, AVFrame* @frame) => vectors.avcodec_send_frame(@avctx, @frame);
    
    /// <summary>Supply raw packet data as input to a decoder.</summary>
    /// <param name="avctx">codec context</param>
    /// <param name="avpkt">The input AVPacket. Usually, this will be a single video frame, or several complete audio frames. Ownership of the packet remains with the caller, and the decoder will not write to the packet. The decoder may create a reference to the packet data (or copy it if the packet is not reference-counted). Unlike with older APIs, the packet is always fully consumed, and if it contains multiple frames (e.g. some audio codecs), will require you to call avcodec_receive_frame() multiple times afterwards before you can send a new packet. It can be NULL (or an AVPacket with data set to NULL and size set to 0); in this case, it is considered a flush packet, which signals the end of the stream. Sending the first flush packet will return success. Subsequent ones are unnecessary and will return AVERROR_EOF. If the decoder still has frames buffered, it will return them after sending a flush packet.</param>
    /// <returns>0 on success, otherwise negative error code: AVERROR(EAGAIN):   input is not accepted in the current state - user must read output with avcodec_receive_frame() (once all output is read, the packet should be resent, and the call will not fail with EAGAIN). AVERROR_EOF:       the decoder has been flushed, and no new packets can be sent to it (also returned if more than 1 flush packet is sent) AVERROR(EINVAL):   codec not opened, it is an encoder, or requires flush AVERROR(ENOMEM):   failed to add packet to internal queue, or similar other errors: legitimate decoding errors</returns>
    public static int avcodec_send_packet(AVCodecContext* @avctx, AVPacket* @avpkt) => vectors.avcodec_send_packet(@avctx, @avpkt);
    
    /// <summary>@}</summary>
    public static void avcodec_string(byte* @buf, int @buf_size, AVCodecContext* @enc, int @encode) => vectors.avcodec_string(@buf, @buf_size, @enc, @encode);
    
    /// <summary>Return the LIBAVCODEC_VERSION_INT constant.</summary>
    public static uint avcodec_version() => vectors.avcodec_version();
    
    /// <summary>Send control message from application to device.</summary>
    /// <param name="s">device context.</param>
    /// <param name="type">message type.</param>
    /// <param name="data">message data. Exact type depends on message type.</param>
    /// <param name="data_size">size of message data.</param>
    /// <returns>&gt;= 0 on success, negative on error. AVERROR(ENOSYS) when device doesn&apos;t implement handler of the message.</returns>
    public static int avdevice_app_to_dev_control_message(AVFormatContext* @s, AVAppToDevMessageType @type, void* @data, ulong @data_size) => vectors.avdevice_app_to_dev_control_message(@s, @type, @data, @data_size);
    
    /// <summary>Initialize capabilities probing API based on AVOption API.</summary>
    /// <param name="caps">Device capabilities data. Pointer to a NULL pointer must be passed.</param>
    /// <param name="s">Context of the device.</param>
    /// <param name="device_options">An AVDictionary filled with device-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL. The same options must be passed later to avformat_write_header() for output devices or avformat_open_input() for input devices, or at any other place that affects device-private options.</param>
    /// <returns>&gt;= 0 on success, negative otherwise.</returns>
    [Obsolete()]
    public static int avdevice_capabilities_create(AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s, AVDictionary** @device_options) => vectors.avdevice_capabilities_create(@caps, @s, @device_options);
    
    /// <summary>Free resources created by avdevice_capabilities_create()</summary>
    /// <param name="caps">Device capabilities data to be freed.</param>
    /// <param name="s">Context of the device.</param>
    [Obsolete()]
    public static void avdevice_capabilities_free(AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s) => vectors.avdevice_capabilities_free(@caps, @s);
    
    /// <summary>Return the libavdevice build-time configuration.</summary>
    public static string avdevice_configuration() => vectors.avdevice_configuration();
    
    /// <summary>Send control message from device to application.</summary>
    /// <param name="s">device context.</param>
    /// <param name="type">message type.</param>
    /// <param name="data">message data. Can be NULL.</param>
    /// <param name="data_size">size of message data.</param>
    /// <returns>&gt;= 0 on success, negative on error. AVERROR(ENOSYS) when application doesn&apos;t implement handler of the message.</returns>
    public static int avdevice_dev_to_app_control_message(AVFormatContext* @s, AVDevToAppMessageType @type, void* @data, ulong @data_size) => vectors.avdevice_dev_to_app_control_message(@s, @type, @data, @data_size);
    
    /// <summary>Convenient function to free result of avdevice_list_devices().</summary>
    public static void avdevice_free_list_devices(AVDeviceInfoList** @device_list) => vectors.avdevice_free_list_devices(@device_list);
    
    /// <summary>Return the libavdevice license.</summary>
    public static string avdevice_license() => vectors.avdevice_license();
    
    /// <summary>List devices.</summary>
    /// <param name="s">device context.</param>
    /// <param name="device_list">list of autodetected devices.</param>
    /// <returns>count of autodetected devices, negative on error.</returns>
    public static int avdevice_list_devices(AVFormatContext* @s, AVDeviceInfoList** @device_list) => vectors.avdevice_list_devices(@s, @device_list);
    
    /// <summary>List devices.</summary>
    /// <param name="device">device format. May be NULL if device name is set.</param>
    /// <param name="device_name">device name. May be NULL if device format is set.</param>
    /// <param name="device_options">An AVDictionary filled with device-private options. May be NULL. The same options must be passed later to avformat_write_header() for output devices or avformat_open_input() for input devices, or at any other place that affects device-private options.</param>
    /// <param name="device_list">list of autodetected devices</param>
    /// <returns>count of autodetected devices, negative on error.</returns>
    public static int avdevice_list_input_sources(AVInputFormat* @device, string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list) => vectors.avdevice_list_input_sources(@device, @device_name, @device_options, @device_list);
    
    public static int avdevice_list_output_sinks(AVOutputFormat* @device, string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list) => vectors.avdevice_list_output_sinks(@device, @device_name, @device_options, @device_list);
    
    /// <summary>Initialize libavdevice and register all the input and output devices.</summary>
    public static void avdevice_register_all() => vectors.avdevice_register_all();
    
    /// <summary>Return the LIBAVDEVICE_VERSION_INT constant.</summary>
    public static uint avdevice_version() => vectors.avdevice_version();
    
    /// <summary>Negotiate the media format, dimensions, etc of all inputs to a filter.</summary>
    /// <param name="filter">the filter to negotiate the properties for its inputs</param>
    /// <returns>zero on successful negotiation</returns>
    public static int avfilter_config_links(AVFilterContext* @filter) => vectors.avfilter_config_links(@filter);
    
    /// <summary>Return the libavfilter build-time configuration.</summary>
    public static string avfilter_configuration() => vectors.avfilter_configuration();
    
    /// <summary>Get the number of elements in an AVFilter&apos;s inputs or outputs array.</summary>
    public static uint avfilter_filter_pad_count(AVFilter* @filter, int @is_output) => vectors.avfilter_filter_pad_count(@filter, @is_output);
    
    /// <summary>Free a filter context. This will also remove the filter from its filtergraph&apos;s list of filters.</summary>
    /// <param name="filter">the filter to free</param>
    public static void avfilter_free(AVFilterContext* @filter) => vectors.avfilter_free(@filter);
    
    /// <summary>Get a filter definition matching the given name.</summary>
    /// <param name="name">the filter name to find</param>
    /// <returns>the filter definition, if any matching one is registered. NULL if none found.</returns>
    public static AVFilter* avfilter_get_by_name(string @name) => vectors.avfilter_get_by_name(@name);
    
    /// <summary>Returns AVClass for AVFilterContext.</summary>
    /// <returns>AVClass for AVFilterContext.</returns>
    public static AVClass* avfilter_get_class() => vectors.avfilter_get_class();
    
    /// <summary>Allocate a filter graph.</summary>
    /// <returns>the allocated filter graph on success or NULL.</returns>
    public static AVFilterGraph* avfilter_graph_alloc() => vectors.avfilter_graph_alloc();
    
    /// <summary>Create a new filter instance in a filter graph.</summary>
    /// <param name="graph">graph in which the new filter will be used</param>
    /// <param name="filter">the filter to create an instance of</param>
    /// <param name="name">Name to give to the new instance (will be copied to AVFilterContext.name). This may be used by the caller to identify different filters, libavfilter itself assigns no semantics to this parameter. May be NULL.</param>
    /// <returns>the context of the newly created filter instance (note that it is also retrievable directly through AVFilterGraph.filters or with avfilter_graph_get_filter()) on success or NULL on failure.</returns>
    public static AVFilterContext* avfilter_graph_alloc_filter(AVFilterGraph* @graph, AVFilter* @filter, string @name) => vectors.avfilter_graph_alloc_filter(@graph, @filter, @name);
    
    /// <summary>Check validity and configure all the links and formats in the graph.</summary>
    /// <param name="graphctx">the filter graph</param>
    /// <param name="log_ctx">context used for logging</param>
    /// <returns>&gt;= 0 in case of success, a negative AVERROR code otherwise</returns>
    public static int avfilter_graph_config(AVFilterGraph* @graphctx, void* @log_ctx) => vectors.avfilter_graph_config(@graphctx, @log_ctx);
    
    /// <summary>Create and add a filter instance into an existing graph. The filter instance is created from the filter filt and inited with the parameter args. opaque is currently ignored.</summary>
    /// <param name="name">the instance name to give to the created filter instance</param>
    /// <param name="graph_ctx">the filter graph</param>
    /// <returns>a negative AVERROR error code in case of failure, a non negative value otherwise</returns>
    public static int avfilter_graph_create_filter(AVFilterContext** @filt_ctx, AVFilter* @filt, string @name, string @args, void* @opaque, AVFilterGraph* @graph_ctx) => vectors.avfilter_graph_create_filter(@filt_ctx, @filt, @name, @args, @opaque, @graph_ctx);
    
    /// <summary>Dump a graph into a human-readable string representation.</summary>
    /// <param name="graph">the graph to dump</param>
    /// <param name="options">formatting options; currently ignored</param>
    /// <returns>a string, or NULL in case of memory allocation failure; the string must be freed using av_free</returns>
    public static byte* avfilter_graph_dump(AVFilterGraph* @graph, string @options) => vectors.avfilter_graph_dump(@graph, @options);
    
    /// <summary>Free a graph, destroy its links, and set *graph to NULL. If *graph is NULL, do nothing.</summary>
    public static void avfilter_graph_free(AVFilterGraph** @graph) => vectors.avfilter_graph_free(@graph);
    
    /// <summary>Get a filter instance identified by instance name from graph.</summary>
    /// <param name="graph">filter graph to search through.</param>
    /// <param name="name">filter instance name (should be unique in the graph).</param>
    /// <returns>the pointer to the found filter instance or NULL if it cannot be found.</returns>
    public static AVFilterContext* avfilter_graph_get_filter(AVFilterGraph* @graph, string @name) => vectors.avfilter_graph_get_filter(@graph, @name);
    
    /// <summary>Add a graph described by a string to a graph.</summary>
    /// <param name="graph">the filter graph where to link the parsed graph context</param>
    /// <param name="filters">string to be parsed</param>
    /// <param name="inputs">linked list to the inputs of the graph</param>
    /// <param name="outputs">linked list to the outputs of the graph</param>
    /// <returns>zero on success, a negative AVERROR code on error</returns>
    public static int avfilter_graph_parse(AVFilterGraph* @graph, string @filters, AVFilterInOut* @inputs, AVFilterInOut* @outputs, void* @log_ctx) => vectors.avfilter_graph_parse(@graph, @filters, @inputs, @outputs, @log_ctx);
    
    /// <summary>Add a graph described by a string to a graph.</summary>
    /// <param name="graph">the filter graph where to link the parsed graph context</param>
    /// <param name="filters">string to be parsed</param>
    /// <param name="inputs">pointer to a linked list to the inputs of the graph, may be NULL. If non-NULL, *inputs is updated to contain the list of open inputs after the parsing, should be freed with avfilter_inout_free().</param>
    /// <param name="outputs">pointer to a linked list to the outputs of the graph, may be NULL. If non-NULL, *outputs is updated to contain the list of open outputs after the parsing, should be freed with avfilter_inout_free().</param>
    /// <returns>non negative on success, a negative AVERROR code on error</returns>
    public static int avfilter_graph_parse_ptr(AVFilterGraph* @graph, string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs, void* @log_ctx) => vectors.avfilter_graph_parse_ptr(@graph, @filters, @inputs, @outputs, @log_ctx);
    
    /// <summary>Add a graph described by a string to a graph.</summary>
    /// <param name="graph">the filter graph where to link the parsed graph context</param>
    /// <param name="filters">string to be parsed</param>
    /// <param name="inputs">a linked list of all free (unlinked) inputs of the parsed graph will be returned here. It is to be freed by the caller using avfilter_inout_free().</param>
    /// <param name="outputs">a linked list of all free (unlinked) outputs of the parsed graph will be returned here. It is to be freed by the caller using avfilter_inout_free().</param>
    /// <returns>zero on success, a negative AVERROR code on error</returns>
    public static int avfilter_graph_parse2(AVFilterGraph* @graph, string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs) => vectors.avfilter_graph_parse2(@graph, @filters, @inputs, @outputs);
    
    /// <summary>Queue a command for one or more filter instances.</summary>
    /// <param name="graph">the filter graph</param>
    /// <param name="target">the filter(s) to which the command should be sent &quot;all&quot; sends to all filters otherwise it can be a filter or filter instance name which will send the command to all matching filters.</param>
    /// <param name="cmd">the command to sent, for handling simplicity all commands must be alphanumeric only</param>
    /// <param name="arg">the argument for the command</param>
    /// <param name="ts">time at which the command should be sent to the filter</param>
    public static int avfilter_graph_queue_command(AVFilterGraph* @graph, string @target, string @cmd, string @arg, int @flags, double @ts) => vectors.avfilter_graph_queue_command(@graph, @target, @cmd, @arg, @flags, @ts);
    
    /// <summary>Request a frame on the oldest sink link.</summary>
    /// <returns>the return value of ff_request_frame(), or AVERROR_EOF if all links returned AVERROR_EOF</returns>
    public static int avfilter_graph_request_oldest(AVFilterGraph* @graph) => vectors.avfilter_graph_request_oldest(@graph);
    
    /// <summary>Send a command to one or more filter instances.</summary>
    /// <param name="graph">the filter graph</param>
    /// <param name="target">the filter(s) to which the command should be sent &quot;all&quot; sends to all filters otherwise it can be a filter or filter instance name which will send the command to all matching filters.</param>
    /// <param name="cmd">the command to send, for handling simplicity all commands must be alphanumeric only</param>
    /// <param name="arg">the argument for the command</param>
    /// <param name="res">a buffer with size res_size where the filter(s) can return a response.</param>
    public static int avfilter_graph_send_command(AVFilterGraph* @graph, string @target, string @cmd, string @arg, byte* @res, int @res_len, int @flags) => vectors.avfilter_graph_send_command(@graph, @target, @cmd, @arg, @res, @res_len, @flags);
    
    /// <summary>Enable or disable automatic format conversion inside the graph.</summary>
    /// <param name="flags">any of the AVFILTER_AUTO_CONVERT_* constants</param>
    public static void avfilter_graph_set_auto_convert(AVFilterGraph* @graph, uint @flags) => vectors.avfilter_graph_set_auto_convert(@graph, @flags);
    
    /// <summary>Initialize a filter with the supplied dictionary of options.</summary>
    /// <param name="ctx">uninitialized filter context to initialize</param>
    /// <param name="options">An AVDictionary filled with options for this filter. On return this parameter will be destroyed and replaced with a dict containing options that were not found. This dictionary must be freed by the caller. May be NULL, then this function is equivalent to avfilter_init_str() with the second parameter set to NULL.</param>
    /// <returns>0 on success, a negative AVERROR on failure</returns>
    public static int avfilter_init_dict(AVFilterContext* @ctx, AVDictionary** @options) => vectors.avfilter_init_dict(@ctx, @options);
    
    /// <summary>Initialize a filter with the supplied parameters.</summary>
    /// <param name="ctx">uninitialized filter context to initialize</param>
    /// <param name="args">Options to initialize the filter with. This must be a &apos;:&apos;-separated list of options in the &apos;key=value&apos; form. May be NULL if the options have been set directly using the AVOptions API or there are no options that need to be set.</param>
    /// <returns>0 on success, a negative AVERROR on failure</returns>
    public static int avfilter_init_str(AVFilterContext* @ctx, string @args) => vectors.avfilter_init_str(@ctx, @args);
    
    /// <summary>Allocate a single AVFilterInOut entry. Must be freed with avfilter_inout_free().</summary>
    /// <returns>allocated AVFilterInOut on success, NULL on failure.</returns>
    public static AVFilterInOut* avfilter_inout_alloc() => vectors.avfilter_inout_alloc();
    
    /// <summary>Free the supplied list of AVFilterInOut and set *inout to NULL. If *inout is NULL, do nothing.</summary>
    public static void avfilter_inout_free(AVFilterInOut** @inout) => vectors.avfilter_inout_free(@inout);
    
    /// <summary>Insert a filter in the middle of an existing link.</summary>
    /// <param name="link">the link into which the filter should be inserted</param>
    /// <param name="filt">the filter to be inserted</param>
    /// <param name="filt_srcpad_idx">the input pad on the filter to connect</param>
    /// <param name="filt_dstpad_idx">the output pad on the filter to connect</param>
    /// <returns>zero on success</returns>
    public static int avfilter_insert_filter(AVFilterLink* @link, AVFilterContext* @filt, uint @filt_srcpad_idx, uint @filt_dstpad_idx) => vectors.avfilter_insert_filter(@link, @filt, @filt_srcpad_idx, @filt_dstpad_idx);
    
    /// <summary>Return the libavfilter license.</summary>
    public static string avfilter_license() => vectors.avfilter_license();
    
    /// <summary>Link two filters together.</summary>
    /// <param name="src">the source filter</param>
    /// <param name="srcpad">index of the output pad on the source filter</param>
    /// <param name="dst">the destination filter</param>
    /// <param name="dstpad">index of the input pad on the destination filter</param>
    /// <returns>zero on success</returns>
    public static int avfilter_link(AVFilterContext* @src, uint @srcpad, AVFilterContext* @dst, uint @dstpad) => vectors.avfilter_link(@src, @srcpad, @dst, @dstpad);
    
    /// <summary>Free the link in *link, and set its pointer to NULL.</summary>
    public static void avfilter_link_free(AVFilterLink** @link) => vectors.avfilter_link_free(@link);
    
    /// <summary>Get the number of elements in an AVFilter&apos;s inputs or outputs array.</summary>
    [Obsolete("Use avfilter_filter_pad_count() instead.")]
    public static int avfilter_pad_count(AVFilterPad* @pads) => vectors.avfilter_pad_count(@pads);
    
    /// <summary>Get the name of an AVFilterPad.</summary>
    /// <param name="pads">an array of AVFilterPads</param>
    /// <param name="pad_idx">index of the pad in the array; it is the caller&apos;s responsibility to ensure the index is valid</param>
    /// <returns>name of the pad_idx&apos;th pad in pads</returns>
    public static string avfilter_pad_get_name(AVFilterPad* @pads, int @pad_idx) => vectors.avfilter_pad_get_name(@pads, @pad_idx);
    
    /// <summary>Get the type of an AVFilterPad.</summary>
    /// <param name="pads">an array of AVFilterPads</param>
    /// <param name="pad_idx">index of the pad in the array; it is the caller&apos;s responsibility to ensure the index is valid</param>
    /// <returns>type of the pad_idx&apos;th pad in pads</returns>
    public static AVMediaType avfilter_pad_get_type(AVFilterPad* @pads, int @pad_idx) => vectors.avfilter_pad_get_type(@pads, @pad_idx);
    
    /// <summary>Make the filter instance process a command. It is recommended to use avfilter_graph_send_command().</summary>
    public static int avfilter_process_command(AVFilterContext* @filter, string @cmd, string @arg, byte* @res, int @res_len, int @flags) => vectors.avfilter_process_command(@filter, @cmd, @arg, @res, @res_len, @flags);
    
    /// <summary>Return the LIBAVFILTER_VERSION_INT constant.</summary>
    public static uint avfilter_version() => vectors.avfilter_version();
    
    /// <summary>Allocate an AVFormatContext. avformat_free_context() can be used to free the context and everything allocated by the framework within it.</summary>
    public static AVFormatContext* avformat_alloc_context() => vectors.avformat_alloc_context();
    
    /// <summary>Allocate an AVFormatContext for an output format. avformat_free_context() can be used to free the context and everything allocated by the framework within it.</summary>
    /// <param name="oformat">format to use for allocating the context, if NULL format_name and filename are used instead</param>
    /// <param name="format_name">the name of output format to use for allocating the context, if NULL filename is used instead</param>
    /// <param name="filename">the name of the filename to use for allocating the context, may be NULL</param>
    /// <returns>&gt;= 0 in case of success, a negative AVERROR code in case of failure</returns>
    public static int avformat_alloc_output_context2(AVFormatContext** @ctx, AVOutputFormat* @oformat, string @format_name, string @filename) => vectors.avformat_alloc_output_context2(@ctx, @oformat, @format_name, @filename);
    
    /// <summary>Close an opened input AVFormatContext. Free it and all its contents and set *s to NULL.</summary>
    public static void avformat_close_input(AVFormatContext** @s) => vectors.avformat_close_input(@s);
    
    /// <summary>Return the libavformat build-time configuration.</summary>
    public static string avformat_configuration() => vectors.avformat_configuration();
    
    /// <summary>Read packets of a media file to get stream information. This is useful for file formats with no headers such as MPEG. This function also computes the real framerate in case of MPEG-2 repeat frame mode. The logical file position is not changed by this function; examined packets may be buffered for later processing.</summary>
    /// <param name="ic">media file handle</param>
    /// <param name="options">If non-NULL, an ic.nb_streams long array of pointers to dictionaries, where i-th member contains options for codec corresponding to i-th stream. On return each dictionary will be filled with options that were not found.</param>
    /// <returns>&gt;=0 if OK, AVERROR_xxx on error</returns>
    public static int avformat_find_stream_info(AVFormatContext* @ic, AVDictionary** @options) => vectors.avformat_find_stream_info(@ic, @options);
    
    /// <summary>Discard all internally buffered data. This can be useful when dealing with discontinuities in the byte stream. Generally works only with formats that can resync. This includes headerless formats like MPEG-TS/TS but should also work with NUT, Ogg and in a limited way AVI for example.</summary>
    /// <param name="s">media file handle</param>
    /// <returns>&gt;=0 on success, error code otherwise</returns>
    public static int avformat_flush(AVFormatContext* @s) => vectors.avformat_flush(@s);
    
    /// <summary>Free an AVFormatContext and all its streams.</summary>
    /// <param name="s">context to free</param>
    public static void avformat_free_context(AVFormatContext* @s) => vectors.avformat_free_context(@s);
    
    /// <summary>Get the AVClass for AVFormatContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* avformat_get_class() => vectors.avformat_get_class();
    
    /// <summary>Returns the table mapping MOV FourCCs for audio to AVCodecID.</summary>
    /// <returns>the table mapping MOV FourCCs for audio to AVCodecID.</returns>
    public static AVCodecTag* avformat_get_mov_audio_tags() => vectors.avformat_get_mov_audio_tags();
    
    /// <summary>Returns the table mapping MOV FourCCs for video to libavcodec AVCodecID.</summary>
    /// <returns>the table mapping MOV FourCCs for video to libavcodec AVCodecID.</returns>
    public static AVCodecTag* avformat_get_mov_video_tags() => vectors.avformat_get_mov_video_tags();
    
    /// <summary>Returns the table mapping RIFF FourCCs for audio to AVCodecID.</summary>
    /// <returns>the table mapping RIFF FourCCs for audio to AVCodecID.</returns>
    public static AVCodecTag* avformat_get_riff_audio_tags() => vectors.avformat_get_riff_audio_tags();
    
    /// <summary>@{ Get the tables mapping RIFF FourCCs to libavcodec AVCodecIDs. The tables are meant to be passed to av_codec_get_id()/av_codec_get_tag() as in the following code:</summary>
    /// <returns>the table mapping RIFF FourCCs for video to libavcodec AVCodecID.</returns>
    public static AVCodecTag* avformat_get_riff_video_tags() => vectors.avformat_get_riff_video_tags();
    
    /// <summary>Get the index entry count for the given AVStream.</summary>
    /// <param name="st">stream</param>
    /// <returns>the number of index entries in the stream</returns>
    public static int avformat_index_get_entries_count(AVStream* @st) => vectors.avformat_index_get_entries_count(@st);
    
    /// <summary>Get the AVIndexEntry corresponding to the given index.</summary>
    /// <param name="st">Stream containing the requested AVIndexEntry.</param>
    /// <param name="idx">The desired index.</param>
    /// <returns>A pointer to the requested AVIndexEntry if it exists, NULL otherwise.</returns>
    public static AVIndexEntry* avformat_index_get_entry(AVStream* @st, int @idx) => vectors.avformat_index_get_entry(@st, @idx);
    
    /// <summary>Get the AVIndexEntry corresponding to the given timestamp.</summary>
    /// <param name="st">Stream containing the requested AVIndexEntry.</param>
    /// <param name="flags">If AVSEEK_FLAG_BACKWARD then the returned entry will correspond to the timestamp which is &lt; = the requested one, if backward is 0, then it will be &gt;= if AVSEEK_FLAG_ANY seek to any frame, only keyframes otherwise.</param>
    /// <returns>A pointer to the requested AVIndexEntry if it exists, NULL otherwise.</returns>
    public static AVIndexEntry* avformat_index_get_entry_from_timestamp(AVStream* @st, long @wanted_timestamp, int @flags) => vectors.avformat_index_get_entry_from_timestamp(@st, @wanted_timestamp, @flags);
    
    /// <summary>Allocate the stream private data and initialize the codec, but do not write the header. May optionally be used before avformat_write_header to initialize stream parameters before actually writing the header. If using this function, do not pass the same options to avformat_write_header.</summary>
    /// <param name="s">Media file handle, must be allocated with avformat_alloc_context(). Its oformat field must be set to the desired output format; Its pb field must be set to an already opened AVIOContext.</param>
    /// <param name="options">An AVDictionary filled with AVFormatContext and muxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>AVSTREAM_INIT_IN_WRITE_HEADER on success if the codec requires avformat_write_header to fully initialize, AVSTREAM_INIT_IN_INIT_OUTPUT  on success if the codec has been fully initialized, negative AVERROR on failure.</returns>
    public static int avformat_init_output(AVFormatContext* @s, AVDictionary** @options) => vectors.avformat_init_output(@s, @options);
    
    /// <summary>Return the libavformat license.</summary>
    public static string avformat_license() => vectors.avformat_license();
    
    /// <summary>Check if the stream st contained in s is matched by the stream specifier spec.</summary>
    /// <returns>&gt;0 if st is matched by spec; 0  if st is not matched by spec; AVERROR code if spec is invalid</returns>
    public static int avformat_match_stream_specifier(AVFormatContext* @s, AVStream* @st, string @spec) => vectors.avformat_match_stream_specifier(@s, @st, @spec);
    
    /// <summary>Undo the initialization done by avformat_network_init. Call it only once for each time you called avformat_network_init.</summary>
    public static int avformat_network_deinit() => vectors.avformat_network_deinit();
    
    /// <summary>Do global initialization of network libraries. This is optional, and not recommended anymore.</summary>
    public static int avformat_network_init() => vectors.avformat_network_init();
    
    /// <summary>Add a new stream to a media file.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="c">unused, does nothing</param>
    /// <returns>newly created stream or NULL on error.</returns>
    public static AVStream* avformat_new_stream(AVFormatContext* @s, AVCodec* @c) => vectors.avformat_new_stream(@s, @c);
    
    /// <summary>Open an input stream and read the header. The codecs are not opened. The stream must be closed with avformat_close_input().</summary>
    /// <param name="ps">Pointer to user-supplied AVFormatContext (allocated by avformat_alloc_context). May be a pointer to NULL, in which case an AVFormatContext is allocated by this function and written into ps. Note that a user-supplied AVFormatContext will be freed on failure.</param>
    /// <param name="url">URL of the stream to open.</param>
    /// <param name="fmt">If non-NULL, this parameter forces a specific input format. Otherwise the format is autodetected.</param>
    /// <param name="options">A dictionary filled with AVFormatContext and demuxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>0 on success, a negative AVERROR on failure.</returns>
    public static int avformat_open_input(AVFormatContext** @ps, string @url, AVInputFormat* @fmt, AVDictionary** @options) => vectors.avformat_open_input(@ps, @url, @fmt, @options);
    
    /// <summary>Test if the given container can store a codec.</summary>
    /// <param name="ofmt">container to check for compatibility</param>
    /// <param name="codec_id">codec to potentially store in container</param>
    /// <param name="std_compliance">standards compliance level, one of FF_COMPLIANCE_*</param>
    /// <returns>1 if codec with ID codec_id can be stored in ofmt, 0 if it cannot. A negative number if this information is not available.</returns>
    public static int avformat_query_codec(AVOutputFormat* @ofmt, AVCodecID @codec_id, int @std_compliance) => vectors.avformat_query_codec(@ofmt, @codec_id, @std_compliance);
    
    public static int avformat_queue_attached_pictures(AVFormatContext* @s) => vectors.avformat_queue_attached_pictures(@s);
    
    /// <summary>Seek to timestamp ts. Seeking will be done so that the point from which all active streams can be presented successfully will be closest to ts and within min/max_ts. Active streams are all streams that have AVStream.discard &lt; AVDISCARD_ALL.</summary>
    /// <param name="s">media file handle</param>
    /// <param name="stream_index">index of the stream which is used as time base reference</param>
    /// <param name="min_ts">smallest acceptable timestamp</param>
    /// <param name="ts">target timestamp</param>
    /// <param name="max_ts">largest acceptable timestamp</param>
    /// <param name="flags">flags</param>
    /// <returns>&gt;=0 on success, error code otherwise</returns>
    public static int avformat_seek_file(AVFormatContext* @s, int @stream_index, long @min_ts, long @ts, long @max_ts, int @flags) => vectors.avformat_seek_file(@s, @stream_index, @min_ts, @ts, @max_ts, @flags);
    
    /// <summary>Transfer internal timing information from one stream to another.</summary>
    /// <param name="ofmt">target output format for ost</param>
    /// <param name="ost">output stream which needs timings copy and adjustments</param>
    /// <param name="ist">reference input stream to copy timings from</param>
    /// <param name="copy_tb">define from where the stream codec timebase needs to be imported</param>
    public static int avformat_transfer_internal_stream_timing_info(AVOutputFormat* @ofmt, AVStream* @ost, AVStream* @ist, AVTimebaseSource @copy_tb) => vectors.avformat_transfer_internal_stream_timing_info(@ofmt, @ost, @ist, @copy_tb);
    
    /// <summary>Return the LIBAVFORMAT_VERSION_INT constant.</summary>
    public static uint avformat_version() => vectors.avformat_version();
    
    /// <summary>Allocate the stream private data and write the stream header to an output media file.</summary>
    /// <param name="s">Media file handle, must be allocated with avformat_alloc_context(). Its oformat field must be set to the desired output format; Its pb field must be set to an already opened AVIOContext.</param>
    /// <param name="options">An AVDictionary filled with AVFormatContext and muxer-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>AVSTREAM_INIT_IN_WRITE_HEADER on success if the codec had not already been fully initialized in avformat_init, AVSTREAM_INIT_IN_INIT_OUTPUT  on success if the codec had already been fully initialized in avformat_init, negative AVERROR on failure.</returns>
    public static int avformat_write_header(AVFormatContext* @s, AVDictionary** @options) => vectors.avformat_write_header(@s, @options);
    
    /// <summary>Accept and allocate a client context on a server context.</summary>
    /// <param name="s">the server context</param>
    /// <param name="c">the client context, must be unallocated</param>
    /// <returns>&gt;= 0 on success or a negative value corresponding to an AVERROR on failure</returns>
    public static int avio_accept(AVIOContext* @s, AVIOContext** @c) => vectors.avio_accept(@s, @c);
    
    /// <summary>Allocate and initialize an AVIOContext for buffered I/O. It must be later freed with avio_context_free().</summary>
    /// <param name="buffer">Memory block for input/output operations via AVIOContext. The buffer must be allocated with av_malloc() and friends. It may be freed and replaced with a new buffer by libavformat. AVIOContext.buffer holds the buffer currently in use, which must be later freed with av_free().</param>
    /// <param name="buffer_size">The buffer size is very important for performance. For protocols with fixed blocksize it should be set to this blocksize. For others a typical size is a cache page, e.g. 4kb.</param>
    /// <param name="write_flag">Set to 1 if the buffer should be writable, 0 otherwise.</param>
    /// <param name="opaque">An opaque pointer to user-specific data.</param>
    /// <param name="read_packet">A function for refilling the buffer, may be NULL. For stream protocols, must never return 0 but rather a proper AVERROR code.</param>
    /// <param name="write_packet">A function for writing the buffer contents, may be NULL. The function may not change the input buffers content.</param>
    /// <param name="seek">A function for seeking to specified byte position, may be NULL.</param>
    /// <returns>Allocated AVIOContext or NULL on failure.</returns>
    public static AVIOContext* avio_alloc_context(byte* @buffer, int @buffer_size, int @write_flag, void* @opaque, avio_alloc_context_read_packet_func @read_packet, avio_alloc_context_write_packet_func @write_packet, avio_alloc_context_seek_func @seek) => vectors.avio_alloc_context(@buffer, @buffer_size, @write_flag, @opaque, @read_packet, @write_packet, @seek);
    
    /// <summary>Return AVIO_FLAG_* access flags corresponding to the access permissions of the resource in url, or a negative value corresponding to an AVERROR code in case of failure. The returned access flags are masked by the value in flags.</summary>
    public static int avio_check(string @url, int @flags) => vectors.avio_check(@url, @flags);
    
    /// <summary>Close the resource accessed by the AVIOContext s and free it. This function can only be used if s was opened by avio_open().</summary>
    /// <returns>0 on success, an AVERROR &lt; 0 on error.</returns>
    public static int avio_close(AVIOContext* @s) => vectors.avio_close(@s);
    
    /// <summary>Close directory.</summary>
    /// <param name="s">directory read context.</param>
    /// <returns>&gt;=0 on success or negative on error.</returns>
    public static int avio_close_dir(AVIODirContext** @s) => vectors.avio_close_dir(@s);
    
    /// <summary>Return the written size and a pointer to the buffer. The buffer must be freed with av_free(). Padding of AV_INPUT_BUFFER_PADDING_SIZE is added to the buffer.</summary>
    /// <param name="s">IO context</param>
    /// <param name="pbuffer">pointer to a byte buffer</param>
    /// <returns>the length of the byte buffer</returns>
    public static int avio_close_dyn_buf(AVIOContext* @s, byte** @pbuffer) => vectors.avio_close_dyn_buf(@s, @pbuffer);
    
    /// <summary>Close the resource accessed by the AVIOContext *s, free it and set the pointer pointing to it to NULL. This function can only be used if s was opened by avio_open().</summary>
    /// <returns>0 on success, an AVERROR &lt; 0 on error.</returns>
    public static int avio_closep(AVIOContext** @s) => vectors.avio_closep(@s);
    
    /// <summary>Free the supplied IO context and everything associated with it.</summary>
    /// <param name="s">Double pointer to the IO context. This function will write NULL into s.</param>
    public static void avio_context_free(AVIOContext** @s) => vectors.avio_context_free(@s);
    
    /// <summary>Iterate through names of available protocols.</summary>
    /// <param name="opaque">A private pointer representing current protocol. It must be a pointer to NULL on first iteration and will be updated by successive calls to avio_enum_protocols.</param>
    /// <param name="output">If set to 1, iterate over output protocols, otherwise over input protocols.</param>
    /// <returns>A static string containing the name of current protocol or NULL</returns>
    public static string avio_enum_protocols(void** @opaque, int @output) => vectors.avio_enum_protocols(@opaque, @output);
    
    /// <summary>Similar to feof() but also returns nonzero on read errors.</summary>
    /// <returns>non zero if and only if at end of file or a read error happened when reading.</returns>
    public static int avio_feof(AVIOContext* @s) => vectors.avio_feof(@s);
    
    /// <summary>Return the name of the protocol that will handle the passed URL.</summary>
    /// <returns>Name of the protocol or NULL.</returns>
    public static string avio_find_protocol_name(string @url) => vectors.avio_find_protocol_name(@url);
    
    /// <summary>Force flushing of buffered data.</summary>
    public static void avio_flush(AVIOContext* @s) => vectors.avio_flush(@s);
    
    /// <summary>Free entry allocated by avio_read_dir().</summary>
    /// <param name="entry">entry to be freed.</param>
    public static void avio_free_directory_entry(AVIODirEntry** @entry) => vectors.avio_free_directory_entry(@entry);
    
    /// <summary>Return the written size and a pointer to the buffer. The AVIOContext stream is left intact. The buffer must NOT be freed. No padding is added to the buffer.</summary>
    /// <param name="s">IO context</param>
    /// <param name="pbuffer">pointer to a byte buffer</param>
    /// <returns>the length of the byte buffer</returns>
    public static int avio_get_dyn_buf(AVIOContext* @s, byte** @pbuffer) => vectors.avio_get_dyn_buf(@s, @pbuffer);
    
    /// <summary>Read a string from pb into buf. The reading will terminate when either a NULL character was encountered, maxlen bytes have been read, or nothing more can be read from pb. The result is guaranteed to be NULL-terminated, it will be truncated if buf is too small. Note that the string is not interpreted or validated in any way, it might get truncated in the middle of a sequence for multi-byte encodings.</summary>
    /// <returns>number of bytes read (is always &lt; = maxlen). If reading ends on EOF or error, the return value will be one more than bytes actually read.</returns>
    public static int avio_get_str(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) => vectors.avio_get_str(@pb, @maxlen, @buf, @buflen);
    
    public static int avio_get_str16be(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) => vectors.avio_get_str16be(@pb, @maxlen, @buf, @buflen);
    
    /// <summary>Read a UTF-16 string from pb and convert it to UTF-8. The reading will terminate when either a null or invalid character was encountered or maxlen bytes have been read.</summary>
    /// <returns>number of bytes read (is always &lt; = maxlen)</returns>
    public static int avio_get_str16le(AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) => vectors.avio_get_str16le(@pb, @maxlen, @buf, @buflen);
    
    /// <summary>Perform one step of the protocol handshake to accept a new client. This function must be called on a client returned by avio_accept() before using it as a read/write context. It is separate from avio_accept() because it may block. A step of the handshake is defined by places where the application may decide to change the proceedings. For example, on a protocol with a request header and a reply header, each one can constitute a step because the application may use the parameters from the request to change parameters in the reply; or each individual chunk of the request can constitute a step. If the handshake is already finished, avio_handshake() does nothing and returns 0 immediately.</summary>
    /// <param name="c">the client context to perform the handshake on</param>
    /// <returns>0   on a complete and successful handshake &gt; 0 if the handshake progressed, but is not complete  &lt; 0 for an AVERROR code</returns>
    public static int avio_handshake(AVIOContext* @c) => vectors.avio_handshake(@c);
    
    /// <summary>Create and initialize a AVIOContext for accessing the resource indicated by url.</summary>
    /// <param name="s">Used to return the pointer to the created AVIOContext. In case of failure the pointed to value is set to NULL.</param>
    /// <param name="url">resource to access</param>
    /// <param name="flags">flags which control how the resource indicated by url is to be opened</param>
    /// <returns>&gt;= 0 in case of success, a negative value corresponding to an AVERROR code in case of failure</returns>
    public static int avio_open(AVIOContext** @s, string @url, int @flags) => vectors.avio_open(@s, @url, @flags);
    
    /// <summary>Open directory for reading.</summary>
    /// <param name="s">directory read context. Pointer to a NULL pointer must be passed.</param>
    /// <param name="url">directory to be listed.</param>
    /// <param name="options">A dictionary filled with protocol-private options. On return this parameter will be destroyed and replaced with a dictionary containing options that were not found. May be NULL.</param>
    /// <returns>&gt;=0 on success or negative on error.</returns>
    public static int avio_open_dir(AVIODirContext** @s, string @url, AVDictionary** @options) => vectors.avio_open_dir(@s, @url, @options);
    
    /// <summary>Open a write only memory stream.</summary>
    /// <param name="s">new IO context</param>
    /// <returns>zero if no error.</returns>
    public static int avio_open_dyn_buf(AVIOContext** @s) => vectors.avio_open_dyn_buf(@s);
    
    /// <summary>Create and initialize a AVIOContext for accessing the resource indicated by url.</summary>
    /// <param name="s">Used to return the pointer to the created AVIOContext. In case of failure the pointed to value is set to NULL.</param>
    /// <param name="url">resource to access</param>
    /// <param name="flags">flags which control how the resource indicated by url is to be opened</param>
    /// <param name="int_cb">an interrupt callback to be used at the protocols level</param>
    /// <param name="options">A dictionary filled with protocol-private options. On return this parameter will be destroyed and replaced with a dict containing options that were not found. May be NULL.</param>
    /// <returns>&gt;= 0 in case of success, a negative value corresponding to an AVERROR code in case of failure</returns>
    public static int avio_open2(AVIOContext** @s, string @url, int @flags, AVIOInterruptCB* @int_cb, AVDictionary** @options) => vectors.avio_open2(@s, @url, @flags, @int_cb, @options);
    
    /// <summary>Pause and resume playing - only meaningful if using a network streaming protocol (e.g. MMS).</summary>
    /// <param name="h">IO context from which to call the read_pause function pointer</param>
    /// <param name="pause">1 for pause, 0 for resume</param>
    public static int avio_pause(AVIOContext* @h, int @pause) => vectors.avio_pause(@h, @pause);
    
    /// <summary>Write a NULL terminated array of strings to the context. Usually you don&apos;t need to use this function directly but its macro wrapper, avio_print.</summary>
    public static void avio_print_string_array(AVIOContext* @s, byte*[] @strings) => vectors.avio_print_string_array(@s, @strings);
    
    /// <summary>Writes a formatted string to the context.</summary>
    /// <returns>number of bytes written, &lt; 0 on error.</returns>
    public static int avio_printf(AVIOContext* @s, string @fmt) => vectors.avio_printf(@s, @fmt);
    
    /// <summary>Get AVClass by names of available protocols.</summary>
    /// <returns>A AVClass of input protocol name or NULL</returns>
    public static AVClass* avio_protocol_get_class(string @name) => vectors.avio_protocol_get_class(@name);
    
    /// <summary>Write a NULL-terminated string.</summary>
    /// <returns>number of bytes written.</returns>
    public static int avio_put_str(AVIOContext* @s, string @str) => vectors.avio_put_str(@s, @str);
    
    /// <summary>Convert an UTF-8 string to UTF-16BE and write it.</summary>
    /// <param name="s">the AVIOContext</param>
    /// <param name="str">NULL-terminated UTF-8 string</param>
    /// <returns>number of bytes written.</returns>
    public static int avio_put_str16be(AVIOContext* @s, string @str) => vectors.avio_put_str16be(@s, @str);
    
    /// <summary>Convert an UTF-8 string to UTF-16LE and write it.</summary>
    /// <param name="s">the AVIOContext</param>
    /// <param name="str">NULL-terminated UTF-8 string</param>
    /// <returns>number of bytes written.</returns>
    public static int avio_put_str16le(AVIOContext* @s, string @str) => vectors.avio_put_str16le(@s, @str);
    
    /// <summary>@{</summary>
    public static int avio_r8(AVIOContext* @s) => vectors.avio_r8(@s);
    
    public static uint avio_rb16(AVIOContext* @s) => vectors.avio_rb16(@s);
    
    public static uint avio_rb24(AVIOContext* @s) => vectors.avio_rb24(@s);
    
    public static uint avio_rb32(AVIOContext* @s) => vectors.avio_rb32(@s);
    
    public static ulong avio_rb64(AVIOContext* @s) => vectors.avio_rb64(@s);
    
    /// <summary>Read size bytes from AVIOContext into buf.</summary>
    /// <returns>number of bytes read or AVERROR</returns>
    public static int avio_read(AVIOContext* @s, byte* @buf, int @size) => vectors.avio_read(@s, @buf, @size);
    
    /// <summary>Get next directory entry.</summary>
    /// <param name="s">directory read context.</param>
    /// <param name="next">next entry or NULL when no more entries.</param>
    /// <returns>&gt;=0 on success or negative on error. End of list is not considered an error.</returns>
    public static int avio_read_dir(AVIODirContext* @s, AVIODirEntry** @next) => vectors.avio_read_dir(@s, @next);
    
    /// <summary>Read size bytes from AVIOContext into buf. Unlike avio_read(), this is allowed to read fewer bytes than requested. The missing bytes can be read in the next call. This always tries to read at least 1 byte. Useful to reduce latency in certain cases.</summary>
    /// <returns>number of bytes read or AVERROR</returns>
    public static int avio_read_partial(AVIOContext* @s, byte* @buf, int @size) => vectors.avio_read_partial(@s, @buf, @size);
    
    /// <summary>Read contents of h into print buffer, up to max_size bytes, or up to EOF.</summary>
    /// <returns>0 for success (max_size bytes read or EOF reached), negative error code otherwise</returns>
    public static int avio_read_to_bprint(AVIOContext* @h, AVBPrint* @pb, ulong @max_size) => vectors.avio_read_to_bprint(@h, @pb, @max_size);
    
    public static uint avio_rl16(AVIOContext* @s) => vectors.avio_rl16(@s);
    
    public static uint avio_rl24(AVIOContext* @s) => vectors.avio_rl24(@s);
    
    public static uint avio_rl32(AVIOContext* @s) => vectors.avio_rl32(@s);
    
    public static ulong avio_rl64(AVIOContext* @s) => vectors.avio_rl64(@s);
    
    /// <summary>fseek() equivalent for AVIOContext.</summary>
    /// <returns>new position or AVERROR.</returns>
    public static long avio_seek(AVIOContext* @s, long @offset, int @whence) => vectors.avio_seek(@s, @offset, @whence);
    
    /// <summary>Seek to a given timestamp relative to some component stream. Only meaningful if using a network streaming protocol (e.g. MMS.).</summary>
    /// <param name="h">IO context from which to call the seek function pointers</param>
    /// <param name="stream_index">The stream index that the timestamp is relative to. If stream_index is (-1) the timestamp should be in AV_TIME_BASE units from the beginning of the presentation. If a stream_index &gt;= 0 is used and the protocol does not support seeking based on component streams, the call will fail.</param>
    /// <param name="timestamp">timestamp in AVStream.time_base units or if there is no stream specified then in AV_TIME_BASE units.</param>
    /// <param name="flags">Optional combination of AVSEEK_FLAG_BACKWARD, AVSEEK_FLAG_BYTE and AVSEEK_FLAG_ANY. The protocol may silently ignore AVSEEK_FLAG_BACKWARD and AVSEEK_FLAG_ANY, but AVSEEK_FLAG_BYTE will fail if used and not supported.</param>
    /// <returns>&gt;= 0 on success</returns>
    public static long avio_seek_time(AVIOContext* @h, int @stream_index, long @timestamp, int @flags) => vectors.avio_seek_time(@h, @stream_index, @timestamp, @flags);
    
    /// <summary>Get the filesize.</summary>
    /// <returns>filesize or AVERROR</returns>
    public static long avio_size(AVIOContext* @s) => vectors.avio_size(@s);
    
    /// <summary>Skip given number of bytes forward</summary>
    /// <returns>new position or AVERROR.</returns>
    public static long avio_skip(AVIOContext* @s, long @offset) => vectors.avio_skip(@s, @offset);
    
    /// <summary>Writes a formatted string to the context taking a va_list.</summary>
    /// <returns>number of bytes written, &lt; 0 on error.</returns>
    public static int avio_vprintf(AVIOContext* @s, string @fmt, byte* @ap) => vectors.avio_vprintf(@s, @fmt, @ap);
    
    public static void avio_w8(AVIOContext* @s, int @b) => vectors.avio_w8(@s, @b);
    
    public static void avio_wb16(AVIOContext* @s, uint @val) => vectors.avio_wb16(@s, @val);
    
    public static void avio_wb24(AVIOContext* @s, uint @val) => vectors.avio_wb24(@s, @val);
    
    public static void avio_wb32(AVIOContext* @s, uint @val) => vectors.avio_wb32(@s, @val);
    
    public static void avio_wb64(AVIOContext* @s, ulong @val) => vectors.avio_wb64(@s, @val);
    
    public static void avio_wl16(AVIOContext* @s, uint @val) => vectors.avio_wl16(@s, @val);
    
    public static void avio_wl24(AVIOContext* @s, uint @val) => vectors.avio_wl24(@s, @val);
    
    public static void avio_wl32(AVIOContext* @s, uint @val) => vectors.avio_wl32(@s, @val);
    
    public static void avio_wl64(AVIOContext* @s, ulong @val) => vectors.avio_wl64(@s, @val);
    
    public static void avio_write(AVIOContext* @s, byte* @buf, int @size) => vectors.avio_write(@s, @buf, @size);
    
    /// <summary>Mark the written bytestream as a specific type.</summary>
    /// <param name="time">the stream time the current bytestream pos corresponds to (in AV_TIME_BASE units), or AV_NOPTS_VALUE if unknown or not applicable</param>
    /// <param name="type">the kind of data written starting at the current pos</param>
    public static void avio_write_marker(AVIOContext* @s, long @time, AVIODataMarkerType @type) => vectors.avio_write_marker(@s, @time, @type);
    
    /// <summary>Free all allocated data in the given subtitle struct.</summary>
    /// <param name="sub">AVSubtitle to free.</param>
    public static void avsubtitle_free(AVSubtitle* @sub) => vectors.avsubtitle_free(@sub);
    
    /// <summary>Return the libavutil build-time configuration.</summary>
    public static string avutil_configuration() => vectors.avutil_configuration();
    
    /// <summary>Return the libavutil license.</summary>
    public static string avutil_license() => vectors.avutil_license();
    
    /// <summary>Return the LIBAVUTIL_VERSION_INT constant.</summary>
    public static uint avutil_version() => vectors.avutil_version();
    
    /// <summary>Return the libpostproc build-time configuration.</summary>
    public static string postproc_configuration() => vectors.postproc_configuration();
    
    /// <summary>Return the libpostproc license.</summary>
    public static string postproc_license() => vectors.postproc_license();
    
    /// <summary>Return the LIBPOSTPROC_VERSION_INT constant.</summary>
    public static uint postproc_version() => vectors.postproc_version();
    
    public static void pp_free_context(void* @ppContext) => vectors.pp_free_context(@ppContext);
    
    public static void pp_free_mode(void* @mode) => vectors.pp_free_mode(@mode);
    
    public static void* pp_get_context(int @width, int @height, int @flags) => vectors.pp_get_context(@width, @height, @flags);
    
    /// <summary>Return a pp_mode or NULL if an error occurred.</summary>
    /// <param name="name">the string after &quot;-pp&quot; on the command line</param>
    /// <param name="quality">a number from 0 to PP_QUALITY_MAX</param>
    public static void* pp_get_mode_by_name_and_quality(string @name, int @quality) => vectors.pp_get_mode_by_name_and_quality(@name, @quality);
    
    public static void pp_postprocess(in byte_ptrArray3 @src, in int_array3 @srcStride, ref byte_ptrArray3 @dst, in int_array3 @dstStride, int @horizontalSize, int @verticalSize, sbyte* @QP_store, int @QP_stride, void* @mode, void* @ppContext, int @pict_type) => vectors.pp_postprocess(@src, @srcStride, ref @dst, @dstStride, @horizontalSize, @verticalSize, @QP_store, @QP_stride, @mode, @ppContext, @pict_type);
    
    /// <summary>Allocate SwrContext.</summary>
    /// <returns>NULL on error, allocated context otherwise</returns>
    public static SwrContext* swr_alloc() => vectors.swr_alloc();
    
    /// <summary>Allocate SwrContext if needed and set/reset common parameters.</summary>
    /// <param name="s">existing Swr context if available, or NULL if not</param>
    /// <param name="out_ch_layout">output channel layout (AV_CH_LAYOUT_*)</param>
    /// <param name="out_sample_fmt">output sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="out_sample_rate">output sample rate (frequency in Hz)</param>
    /// <param name="in_ch_layout">input channel layout (AV_CH_LAYOUT_*)</param>
    /// <param name="in_sample_fmt">input sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="in_sample_rate">input sample rate (frequency in Hz)</param>
    /// <param name="log_offset">logging level offset</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>NULL on error, allocated context otherwise</returns>
    [Obsolete("use ")]
    public static SwrContext* swr_alloc_set_opts(SwrContext* @s, long @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, long @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx) => vectors.swr_alloc_set_opts(@s, @out_ch_layout, @out_sample_fmt, @out_sample_rate, @in_ch_layout, @in_sample_fmt, @in_sample_rate, @log_offset, @log_ctx);
    
    /// <summary>Allocate SwrContext if needed and set/reset common parameters.</summary>
    /// <param name="ps">Pointer to an existing Swr context if available, or to NULL if not. On success, *ps will be set to the allocated context.</param>
    /// <param name="out_ch_layout">output channel layout (e.g. AV_CHANNEL_LAYOUT_*)</param>
    /// <param name="out_sample_fmt">output sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="out_sample_rate">output sample rate (frequency in Hz)</param>
    /// <param name="in_ch_layout">input channel layout (e.g. AV_CHANNEL_LAYOUT_*)</param>
    /// <param name="in_sample_fmt">input sample format (AV_SAMPLE_FMT_*).</param>
    /// <param name="in_sample_rate">input sample rate (frequency in Hz)</param>
    /// <param name="log_offset">logging level offset</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>0 on success, a negative AVERROR code on error. On error, the Swr context is freed and *ps set to NULL.</returns>
    public static int swr_alloc_set_opts2(SwrContext** @ps, AVChannelLayout* @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, AVChannelLayout* @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx) => vectors.swr_alloc_set_opts2(@ps, @out_ch_layout, @out_sample_fmt, @out_sample_rate, @in_ch_layout, @in_sample_fmt, @in_sample_rate, @log_offset, @log_ctx);
    
    /// <summary>Generate a channel mixing matrix.</summary>
    /// <param name="in_layout">input channel layout</param>
    /// <param name="out_layout">output channel layout</param>
    /// <param name="center_mix_level">mix level for the center channel</param>
    /// <param name="surround_mix_level">mix level for the surround channel(s)</param>
    /// <param name="lfe_mix_level">mix level for the low-frequency effects channel</param>
    /// <param name="rematrix_maxval">if 1.0, coefficients will be normalized to prevent overflow. if INT_MAX, coefficients will not be normalized.</param>
    /// <param name="matrix">mixing coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o.</param>
    /// <param name="stride">distance between adjacent input channels in the matrix array</param>
    /// <param name="matrix_encoding">matrixed stereo downmix mode (e.g. dplii)</param>
    /// <param name="log_ctx">parent logging context, can be NULL</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    [Obsolete("use ")]
    public static int swr_build_matrix(ulong @in_layout, ulong @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @rematrix_maxval, double @rematrix_volume, double* @matrix, int @stride, AVMatrixEncoding @matrix_encoding, void* @log_ctx) => vectors.swr_build_matrix(@in_layout, @out_layout, @center_mix_level, @surround_mix_level, @lfe_mix_level, @rematrix_maxval, @rematrix_volume, @matrix, @stride, @matrix_encoding, @log_ctx);
    
    /// <summary>Generate a channel mixing matrix.</summary>
    /// <param name="in_layout">input channel layout</param>
    /// <param name="out_layout">output channel layout</param>
    /// <param name="center_mix_level">mix level for the center channel</param>
    /// <param name="surround_mix_level">mix level for the surround channel(s)</param>
    /// <param name="lfe_mix_level">mix level for the low-frequency effects channel</param>
    /// <param name="matrix">mixing coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o.</param>
    /// <param name="stride">distance between adjacent input channels in the matrix array</param>
    /// <param name="matrix_encoding">matrixed stereo downmix mode (e.g. dplii)</param>
    /// <returns>0 on success, negative AVERROR code on failure</returns>
    public static int swr_build_matrix2(AVChannelLayout* @in_layout, AVChannelLayout* @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @maxval, double @rematrix_volume, double* @matrix, long @stride, AVMatrixEncoding @matrix_encoding, void* @log_context) => vectors.swr_build_matrix2(@in_layout, @out_layout, @center_mix_level, @surround_mix_level, @lfe_mix_level, @maxval, @rematrix_volume, @matrix, @stride, @matrix_encoding, @log_context);
    
    /// <summary>Closes the context so that swr_is_initialized() returns 0.</summary>
    /// <param name="s">Swr context to be closed</param>
    public static void swr_close(SwrContext* @s) => vectors.swr_close(@s);
    
    /// <summary>Configure or reconfigure the SwrContext using the information provided by the AVFrames.</summary>
    /// <param name="swr">audio resample context</param>
    /// <returns>0 on success, AVERROR on failure.</returns>
    public static int swr_config_frame(SwrContext* @swr, AVFrame* @out, AVFrame* @in) => vectors.swr_config_frame(@swr, @out, @in);
    
    /// <summary>Convert audio.</summary>
    /// <param name="s">allocated Swr context, with parameters set</param>
    /// <param name="out">output buffers, only the first one need be set in case of packed audio</param>
    /// <param name="out_count">amount of space available for output in samples per channel</param>
    /// <param name="in">input buffers, only the first one need to be set in case of packed audio</param>
    /// <param name="in_count">number of input samples available in one channel</param>
    /// <returns>number of samples output per channel, negative value on error</returns>
    public static int swr_convert(SwrContext* @s, byte** @out, int @out_count, byte** @in, int @in_count) => vectors.swr_convert(@s, @out, @out_count, @in, @in_count);
    
    /// <summary>Convert the samples in the input AVFrame and write them to the output AVFrame.</summary>
    /// <param name="swr">audio resample context</param>
    /// <param name="output">output AVFrame</param>
    /// <param name="input">input AVFrame</param>
    /// <returns>0 on success, AVERROR on failure or nonmatching configuration.</returns>
    public static int swr_convert_frame(SwrContext* @swr, AVFrame* @output, AVFrame* @input) => vectors.swr_convert_frame(@swr, @output, @input);
    
    /// <summary>Drops the specified number of output samples.</summary>
    /// <param name="s">allocated Swr context</param>
    /// <param name="count">number of samples to be dropped</param>
    /// <returns>&gt;= 0 on success, or a negative AVERROR code on failure</returns>
    public static int swr_drop_output(SwrContext* @s, int @count) => vectors.swr_drop_output(@s, @count);
    
    /// <summary>Free the given SwrContext and set the pointer to NULL.</summary>
    /// <param name="s">a pointer to a pointer to Swr context</param>
    public static void swr_free(SwrContext** @s) => vectors.swr_free(@s);
    
    /// <summary>Get the AVClass for SwrContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    /// <returns>the AVClass of SwrContext</returns>
    public static AVClass* swr_get_class() => vectors.swr_get_class();
    
    /// <summary>Gets the delay the next input sample will experience relative to the next output sample.</summary>
    /// <param name="s">swr context</param>
    /// <param name="base">timebase in which the returned delay will be:</param>
    public static long swr_get_delay(SwrContext* @s, long @base) => vectors.swr_get_delay(@s, @base);
    
    /// <summary>Find an upper bound on the number of samples that the next swr_convert call will output, if called with in_samples of input samples. This depends on the internal state, and anything changing the internal state (like further swr_convert() calls) will may change the number of samples swr_get_out_samples() returns for the same number of input samples.</summary>
    /// <param name="in_samples">number of input samples.</param>
    public static int swr_get_out_samples(SwrContext* @s, int @in_samples) => vectors.swr_get_out_samples(@s, @in_samples);
    
    /// <summary>Initialize context after user parameters have been set.</summary>
    /// <param name="s">Swr context to initialize</param>
    /// <returns>AVERROR error code in case of failure.</returns>
    public static int swr_init(SwrContext* @s) => vectors.swr_init(@s);
    
    /// <summary>Injects the specified number of silence samples.</summary>
    /// <param name="s">allocated Swr context</param>
    /// <param name="count">number of samples to be dropped</param>
    /// <returns>&gt;= 0 on success, or a negative AVERROR code on failure</returns>
    public static int swr_inject_silence(SwrContext* @s, int @count) => vectors.swr_inject_silence(@s, @count);
    
    /// <summary>Check whether an swr context has been initialized or not.</summary>
    /// <param name="s">Swr context to check</param>
    /// <returns>positive if it has been initialized, 0 if not initialized</returns>
    public static int swr_is_initialized(SwrContext* @s) => vectors.swr_is_initialized(@s);
    
    /// <summary>Convert the next timestamp from input to output timestamps are in 1/(in_sample_rate * out_sample_rate) units.</summary>
    /// <returns>the output timestamp for the next output sample</returns>
    public static long swr_next_pts(SwrContext* @s, long @pts) => vectors.swr_next_pts(@s, @pts);
    
    /// <summary>Set a customized input channel mapping.</summary>
    /// <param name="s">allocated Swr context, not yet initialized</param>
    /// <param name="channel_map">customized input channel mapping (array of channel indexes, -1 for a muted channel)</param>
    /// <returns>&gt;= 0 on success, or AVERROR error code in case of failure.</returns>
    public static int swr_set_channel_mapping(SwrContext* @s, int* @channel_map) => vectors.swr_set_channel_mapping(@s, @channel_map);
    
    /// <summary>Activate resampling compensation (&quot;soft&quot; compensation). This function is internally called when needed in swr_next_pts().</summary>
    /// <param name="s">allocated Swr context. If it is not initialized, or SWR_FLAG_RESAMPLE is not set, swr_init() is called with the flag set.</param>
    /// <param name="sample_delta">delta in PTS per sample</param>
    /// <param name="compensation_distance">number of samples to compensate for</param>
    /// <returns>&gt;= 0 on success, AVERROR error codes if:</returns>
    public static int swr_set_compensation(SwrContext* @s, int @sample_delta, int @compensation_distance) => vectors.swr_set_compensation(@s, @sample_delta, @compensation_distance);
    
    /// <summary>Set a customized remix matrix.</summary>
    /// <param name="s">allocated Swr context, not yet initialized</param>
    /// <param name="matrix">remix coefficients; matrix[i + stride * o] is the weight of input channel i in output channel o</param>
    /// <param name="stride">offset between lines of the matrix</param>
    /// <returns>&gt;= 0 on success, or AVERROR error code in case of failure.</returns>
    public static int swr_set_matrix(SwrContext* @s, double* @matrix, int @stride) => vectors.swr_set_matrix(@s, @matrix, @stride);
    
    /// <summary>Return the swr build-time configuration.</summary>
    public static string swresample_configuration() => vectors.swresample_configuration();
    
    /// <summary>Return the swr license.</summary>
    public static string swresample_license() => vectors.swresample_license();
    
    /// <summary>Return the LIBSWRESAMPLE_VERSION_INT constant.</summary>
    public static uint swresample_version() => vectors.swresample_version();
    
    /// <summary>Allocate an empty SwsContext. This must be filled and passed to sws_init_context(). For filling see AVOptions, options.c and sws_setColorspaceDetails().</summary>
    public static SwsContext* sws_alloc_context() => vectors.sws_alloc_context();
    
    /// <summary>Allocate and return an uninitialized vector with length coefficients.</summary>
    public static SwsVector* sws_allocVec(int @length) => vectors.sws_allocVec(@length);
    
    /// <summary>Convert an 8-bit paletted frame into a frame with a color depth of 24 bits.</summary>
    /// <param name="src">source frame buffer</param>
    /// <param name="dst">destination frame buffer</param>
    /// <param name="num_pixels">number of pixels to convert</param>
    /// <param name="palette">array with [256] entries, which must match color arrangement (RGB or BGR) of src</param>
    public static void sws_convertPalette8ToPacked24(byte* @src, byte* @dst, int @num_pixels, byte* @palette) => vectors.sws_convertPalette8ToPacked24(@src, @dst, @num_pixels, @palette);
    
    /// <summary>Convert an 8-bit paletted frame into a frame with a color depth of 32 bits.</summary>
    /// <param name="src">source frame buffer</param>
    /// <param name="dst">destination frame buffer</param>
    /// <param name="num_pixels">number of pixels to convert</param>
    /// <param name="palette">array with [256] entries, which must match color arrangement (RGB or BGR) of src</param>
    public static void sws_convertPalette8ToPacked32(byte* @src, byte* @dst, int @num_pixels, byte* @palette) => vectors.sws_convertPalette8ToPacked32(@src, @dst, @num_pixels, @palette);
    
    /// <summary>Finish the scaling process for a pair of source/destination frames previously submitted with sws_frame_start(). Must be called after all sws_send_slice() and sws_receive_slice() calls are done, before any new sws_frame_start() calls.</summary>
    public static void sws_frame_end(SwsContext* @c) => vectors.sws_frame_end(@c);
    
    /// <summary>Initialize the scaling process for a given pair of source/destination frames. Must be called before any calls to sws_send_slice() and sws_receive_slice().</summary>
    /// <param name="dst">The destination frame.</param>
    /// <param name="src">The source frame. The data buffers must be allocated, but the frame data does not have to be ready at this point. Data availability is then signalled by sws_send_slice().</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int sws_frame_start(SwsContext* @c, AVFrame* @dst, AVFrame* @src) => vectors.sws_frame_start(@c, @dst, @src);
    
    /// <summary>Free the swscaler context swsContext. If swsContext is NULL, then does nothing.</summary>
    public static void sws_freeContext(SwsContext* @swsContext) => vectors.sws_freeContext(@swsContext);
    
    public static void sws_freeFilter(SwsFilter* @filter) => vectors.sws_freeFilter(@filter);
    
    public static void sws_freeVec(SwsVector* @a) => vectors.sws_freeVec(@a);
    
    /// <summary>Get the AVClass for swsContext. It can be used in combination with AV_OPT_SEARCH_FAKE_OBJ for examining options.</summary>
    public static AVClass* sws_get_class() => vectors.sws_get_class();
    
    /// <summary>Check if context can be reused, otherwise reallocate a new one.</summary>
    public static SwsContext* sws_getCachedContext(SwsContext* @context, int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param) => vectors.sws_getCachedContext(@context, @srcW, @srcH, @srcFormat, @dstW, @dstH, @dstFormat, @flags, @srcFilter, @dstFilter, @param);
    
    /// <summary>Return a pointer to yuv&lt;-&gt;rgb coefficients for the given colorspace suitable for sws_setColorspaceDetails().</summary>
    /// <param name="colorspace">One of the SWS_CS_* macros. If invalid, SWS_CS_DEFAULT is used.</param>
    public static int* sws_getCoefficients(int @colorspace) => vectors.sws_getCoefficients(@colorspace);
    
    /// <summary>#if LIBSWSCALE_VERSION_MAJOR &gt; 6</summary>
    /// <returns>negative error code on error, non negative otherwise #else</returns>
    public static int sws_getColorspaceDetails(SwsContext* @c, int** @inv_table, int* @srcRange, int** @table, int* @dstRange, int* @brightness, int* @contrast, int* @saturation) => vectors.sws_getColorspaceDetails(@c, @inv_table, @srcRange, @table, @dstRange, @brightness, @contrast, @saturation);
    
    /// <summary>Allocate and return an SwsContext. You need it to perform scaling/conversion operations using sws_scale().</summary>
    /// <param name="srcW">the width of the source image</param>
    /// <param name="srcH">the height of the source image</param>
    /// <param name="srcFormat">the source image format</param>
    /// <param name="dstW">the width of the destination image</param>
    /// <param name="dstH">the height of the destination image</param>
    /// <param name="dstFormat">the destination image format</param>
    /// <param name="flags">specify which algorithm and options to use for rescaling</param>
    /// <param name="param">extra parameters to tune the used scaler For SWS_BICUBIC param[0] and [1] tune the shape of the basis function, param[0] tunes f(1) and param[1] f´(1) For SWS_GAUSS param[0] tunes the exponent and thus cutoff frequency For SWS_LANCZOS param[0] tunes the width of the window function</param>
    /// <returns>a pointer to an allocated context, or NULL in case of error</returns>
    public static SwsContext* sws_getContext(int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param) => vectors.sws_getContext(@srcW, @srcH, @srcFormat, @dstW, @dstH, @dstFormat, @flags, @srcFilter, @dstFilter, @param);
    
    public static SwsFilter* sws_getDefaultFilter(float @lumaGBlur, float @chromaGBlur, float @lumaSharpen, float @chromaSharpen, float @chromaHShift, float @chromaVShift, int @verbose) => vectors.sws_getDefaultFilter(@lumaGBlur, @chromaGBlur, @lumaSharpen, @chromaSharpen, @chromaHShift, @chromaVShift, @verbose);
    
    /// <summary>Return a normalized Gaussian curve used to filter stuff quality = 3 is high quality, lower is lower quality.</summary>
    public static SwsVector* sws_getGaussianVec(double @variance, double @quality) => vectors.sws_getGaussianVec(@variance, @quality);
    
    /// <summary>Initialize the swscaler context sws_context.</summary>
    /// <returns>zero or positive value on success, a negative value on error</returns>
    public static int sws_init_context(SwsContext* @sws_context, SwsFilter* @srcFilter, SwsFilter* @dstFilter) => vectors.sws_init_context(@sws_context, @srcFilter, @dstFilter);
    
    /// <summary>Returns a positive value if an endianness conversion for pix_fmt is supported, 0 otherwise.</summary>
    /// <param name="pix_fmt">the pixel format</param>
    /// <returns>a positive value if an endianness conversion for pix_fmt is supported, 0 otherwise.</returns>
    public static int sws_isSupportedEndiannessConversion(AVPixelFormat @pix_fmt) => vectors.sws_isSupportedEndiannessConversion(@pix_fmt);
    
    /// <summary>Return a positive value if pix_fmt is a supported input format, 0 otherwise.</summary>
    public static int sws_isSupportedInput(AVPixelFormat @pix_fmt) => vectors.sws_isSupportedInput(@pix_fmt);
    
    /// <summary>Return a positive value if pix_fmt is a supported output format, 0 otherwise.</summary>
    public static int sws_isSupportedOutput(AVPixelFormat @pix_fmt) => vectors.sws_isSupportedOutput(@pix_fmt);
    
    /// <summary>Scale all the coefficients of a so that their sum equals height.</summary>
    public static void sws_normalizeVec(SwsVector* @a, double @height) => vectors.sws_normalizeVec(@a, @height);
    
    /// <summary>Request a horizontal slice of the output data to be written into the frame previously provided to sws_frame_start().</summary>
    /// <param name="slice_start">first row of the slice; must be a multiple of sws_receive_slice_alignment()</param>
    /// <param name="slice_height">number of rows in the slice; must be a multiple of sws_receive_slice_alignment(), except for the last slice (i.e. when slice_start+slice_height is equal to output frame height)</param>
    /// <returns>a non-negative number if the data was successfully written into the output AVERROR(EAGAIN) if more input data needs to be provided before the output can be produced another negative AVERROR code on other kinds of scaling failure</returns>
    public static int sws_receive_slice(SwsContext* @c, uint @slice_start, uint @slice_height) => vectors.sws_receive_slice(@c, @slice_start, @slice_height);
    
    /// <summary>Returns alignment required for output slices requested with sws_receive_slice(). Slice offsets and sizes passed to sws_receive_slice() must be multiples of the value returned from this function.</summary>
    /// <returns>alignment required for output slices requested with sws_receive_slice(). Slice offsets and sizes passed to sws_receive_slice() must be multiples of the value returned from this function.</returns>
    public static uint sws_receive_slice_alignment(SwsContext* @c) => vectors.sws_receive_slice_alignment(@c);
    
    /// <summary>Scale the image slice in srcSlice and put the resulting scaled slice in the image in dst. A slice is a sequence of consecutive rows in an image.</summary>
    /// <param name="c">the scaling context previously created with sws_getContext()</param>
    /// <param name="srcSlice">the array containing the pointers to the planes of the source slice</param>
    /// <param name="srcStride">the array containing the strides for each plane of the source image</param>
    /// <param name="srcSliceY">the position in the source image of the slice to process, that is the number (counted starting from zero) in the image of the first row of the slice</param>
    /// <param name="srcSliceH">the height of the source slice, that is the number of rows in the slice</param>
    /// <param name="dst">the array containing the pointers to the planes of the destination image</param>
    /// <param name="dstStride">the array containing the strides for each plane of the destination image</param>
    /// <returns>the height of the output slice</returns>
    public static int sws_scale(SwsContext* @c, byte*[] @srcSlice, int[] @srcStride, int @srcSliceY, int @srcSliceH, byte*[] @dst, int[] @dstStride) => vectors.sws_scale(@c, @srcSlice, @srcStride, @srcSliceY, @srcSliceH, @dst, @dstStride);
    
    /// <summary>Scale source data from src and write the output to dst.</summary>
    /// <param name="dst">The destination frame. See documentation for sws_frame_start() for more details.</param>
    /// <param name="src">The source frame.</param>
    /// <returns>0 on success, a negative AVERROR code on failure</returns>
    public static int sws_scale_frame(SwsContext* @c, AVFrame* @dst, AVFrame* @src) => vectors.sws_scale_frame(@c, @dst, @src);
    
    /// <summary>Scale all the coefficients of a by the scalar value.</summary>
    public static void sws_scaleVec(SwsVector* @a, double @scalar) => vectors.sws_scaleVec(@a, @scalar);
    
    /// <summary>Indicate that a horizontal slice of input data is available in the source frame previously provided to sws_frame_start(). The slices may be provided in any order, but may not overlap. For vertically subsampled pixel formats, the slices must be aligned according to subsampling.</summary>
    /// <param name="slice_start">first row of the slice</param>
    /// <param name="slice_height">number of rows in the slice</param>
    /// <returns>a non-negative number on success, a negative AVERROR code on failure.</returns>
    public static int sws_send_slice(SwsContext* @c, uint @slice_start, uint @slice_height) => vectors.sws_send_slice(@c, @slice_start, @slice_height);
    
    /// <summary>Returns negative error code on error, non negative otherwise #else Returns -1 if not supported #endif</summary>
    /// <param name="inv_table">the yuv2rgb coefficients describing the input yuv space, normally ff_yuv2rgb_coeffs[x]</param>
    /// <param name="srcRange">flag indicating the while-black range of the input (1=jpeg / 0=mpeg)</param>
    /// <param name="table">the yuv2rgb coefficients describing the output yuv space, normally ff_yuv2rgb_coeffs[x]</param>
    /// <param name="dstRange">flag indicating the while-black range of the output (1=jpeg / 0=mpeg)</param>
    /// <param name="brightness">16.16 fixed point brightness correction</param>
    /// <param name="contrast">16.16 fixed point contrast correction</param>
    /// <param name="saturation">16.16 fixed point saturation correction #if LIBSWSCALE_VERSION_MAJOR &gt; 6</param>
    /// <returns>negative error code on error, non negative otherwise #else</returns>
    public static int sws_setColorspaceDetails(SwsContext* @c, in int_array4 @inv_table, int @srcRange, in int_array4 @table, int @dstRange, int @brightness, int @contrast, int @saturation) => vectors.sws_setColorspaceDetails(@c, @inv_table, @srcRange, @table, @dstRange, @brightness, @contrast, @saturation);
    
    /// <summary>Return the libswscale build-time configuration.</summary>
    public static string swscale_configuration() => vectors.swscale_configuration();
    
    /// <summary>Return the libswscale license.</summary>
    public static string swscale_license() => vectors.swscale_license();
    
    /// <summary>Color conversion and scaling library.</summary>
    public static uint swscale_version() => vectors.swscale_version();
    
}
