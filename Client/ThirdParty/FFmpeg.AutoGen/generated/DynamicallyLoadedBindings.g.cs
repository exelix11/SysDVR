using System;
using System.Runtime.InteropServices;

namespace FFmpeg.AutoGen;

public static unsafe partial class DynamicallyLoadedBindings
{
    public static bool ThrowErrorIfFunctionNotFound;
    public static IFunctionResolver FunctionResolver;
    
    public unsafe static void Initialize()
    {
        if (FunctionResolver == null) FunctionResolver = FunctionResolverFactory.Create();
        
        vectors.av_abuffersink_params_alloc = () =>
        {
            vectors.av_abuffersink_params_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_abuffersink_params_alloc_delegate>("avfilter", "av_abuffersink_params_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_abuffersink_params_alloc();
        };
        
        vectors.av_add_index_entry = (AVStream* @st, long @pos, long @timestamp, int @size, int @distance, int @flags) =>
        {
            vectors.av_add_index_entry = FunctionResolver.GetFunctionDelegate<vectors.av_add_index_entry_delegate>("avformat", "av_add_index_entry", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_add_index_entry(@st, @pos, @timestamp, @size, @distance, @flags);
        };
        
        vectors.av_add_q = (AVRational @b, AVRational @c) =>
        {
            vectors.av_add_q = FunctionResolver.GetFunctionDelegate<vectors.av_add_q_delegate>("avutil", "av_add_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_add_q(@b, @c);
        };
        
        vectors.av_add_stable = (AVRational @ts_tb, long @ts, AVRational @inc_tb, long @inc) =>
        {
            vectors.av_add_stable = FunctionResolver.GetFunctionDelegate<vectors.av_add_stable_delegate>("avutil", "av_add_stable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_add_stable(@ts_tb, @ts, @inc_tb, @inc);
        };
        
        vectors.av_append_packet = (AVIOContext* @s, AVPacket* @pkt, int @size) =>
        {
            vectors.av_append_packet = FunctionResolver.GetFunctionDelegate<vectors.av_append_packet_delegate>("avformat", "av_append_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_append_packet(@s, @pkt, @size);
        };
        
        vectors.av_audio_fifo_alloc = (AVSampleFormat @sample_fmt, int @channels, int @nb_samples) =>
        {
            vectors.av_audio_fifo_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_alloc_delegate>("avutil", "av_audio_fifo_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_alloc(@sample_fmt, @channels, @nb_samples);
        };
        
        vectors.av_audio_fifo_drain = (AVAudioFifo* @af, int @nb_samples) =>
        {
            vectors.av_audio_fifo_drain = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_drain_delegate>("avutil", "av_audio_fifo_drain", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_drain(@af, @nb_samples);
        };
        
        vectors.av_audio_fifo_free = (AVAudioFifo* @af) =>
        {
            vectors.av_audio_fifo_free = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_free_delegate>("avutil", "av_audio_fifo_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_audio_fifo_free(@af);
        };
        
        vectors.av_audio_fifo_peek = (AVAudioFifo* @af, void** @data, int @nb_samples) =>
        {
            vectors.av_audio_fifo_peek = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_peek_delegate>("avutil", "av_audio_fifo_peek", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_peek(@af, @data, @nb_samples);
        };
        
        vectors.av_audio_fifo_peek_at = (AVAudioFifo* @af, void** @data, int @nb_samples, int @offset) =>
        {
            vectors.av_audio_fifo_peek_at = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_peek_at_delegate>("avutil", "av_audio_fifo_peek_at", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_peek_at(@af, @data, @nb_samples, @offset);
        };
        
        vectors.av_audio_fifo_read = (AVAudioFifo* @af, void** @data, int @nb_samples) =>
        {
            vectors.av_audio_fifo_read = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_read_delegate>("avutil", "av_audio_fifo_read", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_read(@af, @data, @nb_samples);
        };
        
        vectors.av_audio_fifo_realloc = (AVAudioFifo* @af, int @nb_samples) =>
        {
            vectors.av_audio_fifo_realloc = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_realloc_delegate>("avutil", "av_audio_fifo_realloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_realloc(@af, @nb_samples);
        };
        
        vectors.av_audio_fifo_reset = (AVAudioFifo* @af) =>
        {
            vectors.av_audio_fifo_reset = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_reset_delegate>("avutil", "av_audio_fifo_reset", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_audio_fifo_reset(@af);
        };
        
        vectors.av_audio_fifo_size = (AVAudioFifo* @af) =>
        {
            vectors.av_audio_fifo_size = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_size_delegate>("avutil", "av_audio_fifo_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_size(@af);
        };
        
        vectors.av_audio_fifo_space = (AVAudioFifo* @af) =>
        {
            vectors.av_audio_fifo_space = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_space_delegate>("avutil", "av_audio_fifo_space", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_space(@af);
        };
        
        vectors.av_audio_fifo_write = (AVAudioFifo* @af, void** @data, int @nb_samples) =>
        {
            vectors.av_audio_fifo_write = FunctionResolver.GetFunctionDelegate<vectors.av_audio_fifo_write_delegate>("avutil", "av_audio_fifo_write", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_audio_fifo_write(@af, @data, @nb_samples);
        };
        
        vectors.av_bprint_channel_layout = (AVBPrint* @bp, int @nb_channels, ulong @channel_layout) =>
        {
            vectors.av_bprint_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_bprint_channel_layout_delegate>("avutil", "av_bprint_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_bprint_channel_layout(@bp, @nb_channels, @channel_layout);
        };
        
        vectors.av_bsf_alloc = (AVBitStreamFilter* @filter, AVBSFContext** @ctx) =>
        {
            vectors.av_bsf_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_alloc_delegate>("avcodec", "av_bsf_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_alloc(@filter, @ctx);
        };
        
        vectors.av_bsf_flush = (AVBSFContext* @ctx) =>
        {
            vectors.av_bsf_flush = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_flush_delegate>("avcodec", "av_bsf_flush", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_bsf_flush(@ctx);
        };
        
        vectors.av_bsf_free = (AVBSFContext** @ctx) =>
        {
            vectors.av_bsf_free = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_free_delegate>("avcodec", "av_bsf_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_bsf_free(@ctx);
        };
        
        vectors.av_bsf_get_by_name = (string @name) =>
        {
            vectors.av_bsf_get_by_name = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_get_by_name_delegate>("avcodec", "av_bsf_get_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_get_by_name(@name);
        };
        
        vectors.av_bsf_get_class = () =>
        {
            vectors.av_bsf_get_class = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_get_class_delegate>("avcodec", "av_bsf_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_get_class();
        };
        
        vectors.av_bsf_get_null_filter = (AVBSFContext** @bsf) =>
        {
            vectors.av_bsf_get_null_filter = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_get_null_filter_delegate>("avcodec", "av_bsf_get_null_filter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_get_null_filter(@bsf);
        };
        
        vectors.av_bsf_init = (AVBSFContext* @ctx) =>
        {
            vectors.av_bsf_init = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_init_delegate>("avcodec", "av_bsf_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_init(@ctx);
        };
        
        vectors.av_bsf_iterate = (void** @opaque) =>
        {
            vectors.av_bsf_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_iterate_delegate>("avcodec", "av_bsf_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_iterate(@opaque);
        };
        
        vectors.av_bsf_list_alloc = () =>
        {
            vectors.av_bsf_list_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_alloc_delegate>("avcodec", "av_bsf_list_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_list_alloc();
        };
        
        vectors.av_bsf_list_append = (AVBSFList* @lst, AVBSFContext* @bsf) =>
        {
            vectors.av_bsf_list_append = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_append_delegate>("avcodec", "av_bsf_list_append", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_list_append(@lst, @bsf);
        };
        
        vectors.av_bsf_list_append2 = (AVBSFList* @lst, string @bsf_name, AVDictionary** @options) =>
        {
            vectors.av_bsf_list_append2 = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_append2_delegate>("avcodec", "av_bsf_list_append2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_list_append2(@lst, @bsf_name, @options);
        };
        
        vectors.av_bsf_list_finalize = (AVBSFList** @lst, AVBSFContext** @bsf) =>
        {
            vectors.av_bsf_list_finalize = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_finalize_delegate>("avcodec", "av_bsf_list_finalize", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_list_finalize(@lst, @bsf);
        };
        
        vectors.av_bsf_list_free = (AVBSFList** @lst) =>
        {
            vectors.av_bsf_list_free = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_free_delegate>("avcodec", "av_bsf_list_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_bsf_list_free(@lst);
        };
        
        vectors.av_bsf_list_parse_str = (string @str, AVBSFContext** @bsf) =>
        {
            vectors.av_bsf_list_parse_str = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_list_parse_str_delegate>("avcodec", "av_bsf_list_parse_str", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_list_parse_str(@str, @bsf);
        };
        
        vectors.av_bsf_receive_packet = (AVBSFContext* @ctx, AVPacket* @pkt) =>
        {
            vectors.av_bsf_receive_packet = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_receive_packet_delegate>("avcodec", "av_bsf_receive_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_receive_packet(@ctx, @pkt);
        };
        
        vectors.av_bsf_send_packet = (AVBSFContext* @ctx, AVPacket* @pkt) =>
        {
            vectors.av_bsf_send_packet = FunctionResolver.GetFunctionDelegate<vectors.av_bsf_send_packet_delegate>("avcodec", "av_bsf_send_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_bsf_send_packet(@ctx, @pkt);
        };
        
        vectors.av_buffer_alloc = (ulong @size) =>
        {
            vectors.av_buffer_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_alloc_delegate>("avutil", "av_buffer_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_alloc(@size);
        };
        
        vectors.av_buffer_allocz = (ulong @size) =>
        {
            vectors.av_buffer_allocz = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_allocz_delegate>("avutil", "av_buffer_allocz", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_allocz(@size);
        };
        
        vectors.av_buffer_create = (byte* @data, ulong @size, av_buffer_create_free_func @free, void* @opaque, int @flags) =>
        {
            vectors.av_buffer_create = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_create_delegate>("avutil", "av_buffer_create", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_create(@data, @size, @free, @opaque, @flags);
        };
        
        vectors.av_buffer_default_free = (void* @opaque, byte* @data) =>
        {
            vectors.av_buffer_default_free = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_default_free_delegate>("avutil", "av_buffer_default_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_buffer_default_free(@opaque, @data);
        };
        
        vectors.av_buffer_get_opaque = (AVBufferRef* @buf) =>
        {
            vectors.av_buffer_get_opaque = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_get_opaque_delegate>("avutil", "av_buffer_get_opaque", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_get_opaque(@buf);
        };
        
        vectors.av_buffer_get_ref_count = (AVBufferRef* @buf) =>
        {
            vectors.av_buffer_get_ref_count = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_get_ref_count_delegate>("avutil", "av_buffer_get_ref_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_get_ref_count(@buf);
        };
        
        vectors.av_buffer_is_writable = (AVBufferRef* @buf) =>
        {
            vectors.av_buffer_is_writable = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_is_writable_delegate>("avutil", "av_buffer_is_writable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_is_writable(@buf);
        };
        
        vectors.av_buffer_make_writable = (AVBufferRef** @buf) =>
        {
            vectors.av_buffer_make_writable = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_make_writable_delegate>("avutil", "av_buffer_make_writable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_make_writable(@buf);
        };
        
        vectors.av_buffer_pool_buffer_get_opaque = (AVBufferRef* @ref) =>
        {
            vectors.av_buffer_pool_buffer_get_opaque = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_pool_buffer_get_opaque_delegate>("avutil", "av_buffer_pool_buffer_get_opaque", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_pool_buffer_get_opaque(@ref);
        };
        
        vectors.av_buffer_pool_get = (AVBufferPool* @pool) =>
        {
            vectors.av_buffer_pool_get = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_pool_get_delegate>("avutil", "av_buffer_pool_get", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_pool_get(@pool);
        };
        
        vectors.av_buffer_pool_init = (ulong @size, av_buffer_pool_init_alloc_func @alloc) =>
        {
            vectors.av_buffer_pool_init = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_pool_init_delegate>("avutil", "av_buffer_pool_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_pool_init(@size, @alloc);
        };
        
        vectors.av_buffer_pool_init2 = (ulong @size, void* @opaque, av_buffer_pool_init2_alloc_func @alloc, av_buffer_pool_init2_pool_free_func @pool_free) =>
        {
            vectors.av_buffer_pool_init2 = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_pool_init2_delegate>("avutil", "av_buffer_pool_init2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_pool_init2(@size, @opaque, @alloc, @pool_free);
        };
        
        vectors.av_buffer_pool_uninit = (AVBufferPool** @pool) =>
        {
            vectors.av_buffer_pool_uninit = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_pool_uninit_delegate>("avutil", "av_buffer_pool_uninit", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_buffer_pool_uninit(@pool);
        };
        
        vectors.av_buffer_realloc = (AVBufferRef** @buf, ulong @size) =>
        {
            vectors.av_buffer_realloc = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_realloc_delegate>("avutil", "av_buffer_realloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_realloc(@buf, @size);
        };
        
        vectors.av_buffer_ref = (AVBufferRef* @buf) =>
        {
            vectors.av_buffer_ref = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_ref_delegate>("avutil", "av_buffer_ref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_ref(@buf);
        };
        
        vectors.av_buffer_replace = (AVBufferRef** @dst, AVBufferRef* @src) =>
        {
            vectors.av_buffer_replace = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_replace_delegate>("avutil", "av_buffer_replace", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffer_replace(@dst, @src);
        };
        
        vectors.av_buffer_unref = (AVBufferRef** @buf) =>
        {
            vectors.av_buffer_unref = FunctionResolver.GetFunctionDelegate<vectors.av_buffer_unref_delegate>("avutil", "av_buffer_unref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_buffer_unref(@buf);
        };
        
        vectors.av_buffersink_get_ch_layout = (AVFilterContext* @ctx, AVChannelLayout* @ch_layout) =>
        {
            vectors.av_buffersink_get_ch_layout = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_ch_layout_delegate>("avfilter", "av_buffersink_get_ch_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_ch_layout(@ctx, @ch_layout);
        };
        
        vectors.av_buffersink_get_channel_layout = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_channel_layout_delegate>("avfilter", "av_buffersink_get_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_channel_layout(@ctx);
        };
        
        vectors.av_buffersink_get_channels = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_channels = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_channels_delegate>("avfilter", "av_buffersink_get_channels", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_channels(@ctx);
        };
        
        vectors.av_buffersink_get_format = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_format = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_format_delegate>("avfilter", "av_buffersink_get_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_format(@ctx);
        };
        
        vectors.av_buffersink_get_frame = (AVFilterContext* @ctx, AVFrame* @frame) =>
        {
            vectors.av_buffersink_get_frame = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_frame_delegate>("avfilter", "av_buffersink_get_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_frame(@ctx, @frame);
        };
        
        vectors.av_buffersink_get_frame_flags = (AVFilterContext* @ctx, AVFrame* @frame, int @flags) =>
        {
            vectors.av_buffersink_get_frame_flags = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_frame_flags_delegate>("avfilter", "av_buffersink_get_frame_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_frame_flags(@ctx, @frame, @flags);
        };
        
        vectors.av_buffersink_get_frame_rate = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_frame_rate = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_frame_rate_delegate>("avfilter", "av_buffersink_get_frame_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_frame_rate(@ctx);
        };
        
        vectors.av_buffersink_get_h = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_h = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_h_delegate>("avfilter", "av_buffersink_get_h", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_h(@ctx);
        };
        
        vectors.av_buffersink_get_hw_frames_ctx = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_hw_frames_ctx = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_hw_frames_ctx_delegate>("avfilter", "av_buffersink_get_hw_frames_ctx", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_hw_frames_ctx(@ctx);
        };
        
        vectors.av_buffersink_get_sample_aspect_ratio = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_sample_aspect_ratio = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_sample_aspect_ratio_delegate>("avfilter", "av_buffersink_get_sample_aspect_ratio", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_sample_aspect_ratio(@ctx);
        };
        
        vectors.av_buffersink_get_sample_rate = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_sample_rate = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_sample_rate_delegate>("avfilter", "av_buffersink_get_sample_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_sample_rate(@ctx);
        };
        
        vectors.av_buffersink_get_samples = (AVFilterContext* @ctx, AVFrame* @frame, int @nb_samples) =>
        {
            vectors.av_buffersink_get_samples = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_samples_delegate>("avfilter", "av_buffersink_get_samples", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_samples(@ctx, @frame, @nb_samples);
        };
        
        vectors.av_buffersink_get_time_base = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_time_base = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_time_base_delegate>("avfilter", "av_buffersink_get_time_base", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_time_base(@ctx);
        };
        
        vectors.av_buffersink_get_type = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_type = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_type_delegate>("avfilter", "av_buffersink_get_type", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_type(@ctx);
        };
        
        vectors.av_buffersink_get_w = (AVFilterContext* @ctx) =>
        {
            vectors.av_buffersink_get_w = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_get_w_delegate>("avfilter", "av_buffersink_get_w", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_get_w(@ctx);
        };
        
        vectors.av_buffersink_params_alloc = () =>
        {
            vectors.av_buffersink_params_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_params_alloc_delegate>("avfilter", "av_buffersink_params_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersink_params_alloc();
        };
        
        vectors.av_buffersink_set_frame_size = (AVFilterContext* @ctx, uint @frame_size) =>
        {
            vectors.av_buffersink_set_frame_size = FunctionResolver.GetFunctionDelegate<vectors.av_buffersink_set_frame_size_delegate>("avfilter", "av_buffersink_set_frame_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_buffersink_set_frame_size(@ctx, @frame_size);
        };
        
        vectors.av_buffersrc_add_frame = (AVFilterContext* @ctx, AVFrame* @frame) =>
        {
            vectors.av_buffersrc_add_frame = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_add_frame_delegate>("avfilter", "av_buffersrc_add_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_add_frame(@ctx, @frame);
        };
        
        vectors.av_buffersrc_add_frame_flags = (AVFilterContext* @buffer_src, AVFrame* @frame, int @flags) =>
        {
            vectors.av_buffersrc_add_frame_flags = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_add_frame_flags_delegate>("avfilter", "av_buffersrc_add_frame_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_add_frame_flags(@buffer_src, @frame, @flags);
        };
        
        vectors.av_buffersrc_close = (AVFilterContext* @ctx, long @pts, uint @flags) =>
        {
            vectors.av_buffersrc_close = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_close_delegate>("avfilter", "av_buffersrc_close", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_close(@ctx, @pts, @flags);
        };
        
        vectors.av_buffersrc_get_nb_failed_requests = (AVFilterContext* @buffer_src) =>
        {
            vectors.av_buffersrc_get_nb_failed_requests = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_get_nb_failed_requests_delegate>("avfilter", "av_buffersrc_get_nb_failed_requests", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_get_nb_failed_requests(@buffer_src);
        };
        
        vectors.av_buffersrc_parameters_alloc = () =>
        {
            vectors.av_buffersrc_parameters_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_parameters_alloc_delegate>("avfilter", "av_buffersrc_parameters_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_parameters_alloc();
        };
        
        vectors.av_buffersrc_parameters_set = (AVFilterContext* @ctx, AVBufferSrcParameters* @param) =>
        {
            vectors.av_buffersrc_parameters_set = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_parameters_set_delegate>("avfilter", "av_buffersrc_parameters_set", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_parameters_set(@ctx, @param);
        };
        
        vectors.av_buffersrc_write_frame = (AVFilterContext* @ctx, AVFrame* @frame) =>
        {
            vectors.av_buffersrc_write_frame = FunctionResolver.GetFunctionDelegate<vectors.av_buffersrc_write_frame_delegate>("avfilter", "av_buffersrc_write_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_buffersrc_write_frame(@ctx, @frame);
        };
        
        vectors.av_calloc = (ulong @nmemb, ulong @size) =>
        {
            vectors.av_calloc = FunctionResolver.GetFunctionDelegate<vectors.av_calloc_delegate>("avutil", "av_calloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_calloc(@nmemb, @size);
        };
        
        vectors.av_channel_description = (byte* @buf, ulong @buf_size, AVChannel @channel) =>
        {
            vectors.av_channel_description = FunctionResolver.GetFunctionDelegate<vectors.av_channel_description_delegate>("avutil", "av_channel_description", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_description(@buf, @buf_size, @channel);
        };
        
        vectors.av_channel_description_bprint = (AVBPrint* @bp, AVChannel @channel_id) =>
        {
            vectors.av_channel_description_bprint = FunctionResolver.GetFunctionDelegate<vectors.av_channel_description_bprint_delegate>("avutil", "av_channel_description_bprint", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_channel_description_bprint(@bp, @channel_id);
        };
        
        vectors.av_channel_from_string = (string @name) =>
        {
            vectors.av_channel_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_channel_from_string_delegate>("avutil", "av_channel_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_from_string(@name);
        };
        
        vectors.av_channel_layout_channel_from_index = (AVChannelLayout* @channel_layout, uint @idx) =>
        {
            vectors.av_channel_layout_channel_from_index = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_channel_from_index_delegate>("avutil", "av_channel_layout_channel_from_index", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_channel_from_index(@channel_layout, @idx);
        };
        
        vectors.av_channel_layout_channel_from_string = (AVChannelLayout* @channel_layout, string @name) =>
        {
            vectors.av_channel_layout_channel_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_channel_from_string_delegate>("avutil", "av_channel_layout_channel_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_channel_from_string(@channel_layout, @name);
        };
        
        vectors.av_channel_layout_check = (AVChannelLayout* @channel_layout) =>
        {
            vectors.av_channel_layout_check = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_check_delegate>("avutil", "av_channel_layout_check", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_check(@channel_layout);
        };
        
        vectors.av_channel_layout_compare = (AVChannelLayout* @chl, AVChannelLayout* @chl1) =>
        {
            vectors.av_channel_layout_compare = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_compare_delegate>("avutil", "av_channel_layout_compare", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_compare(@chl, @chl1);
        };
        
        vectors.av_channel_layout_copy = (AVChannelLayout* @dst, AVChannelLayout* @src) =>
        {
            vectors.av_channel_layout_copy = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_copy_delegate>("avutil", "av_channel_layout_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_copy(@dst, @src);
        };
        
        vectors.av_channel_layout_default = (AVChannelLayout* @ch_layout, int @nb_channels) =>
        {
            vectors.av_channel_layout_default = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_default_delegate>("avutil", "av_channel_layout_default", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_channel_layout_default(@ch_layout, @nb_channels);
        };
        
        vectors.av_channel_layout_describe = (AVChannelLayout* @channel_layout, byte* @buf, ulong @buf_size) =>
        {
            vectors.av_channel_layout_describe = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_describe_delegate>("avutil", "av_channel_layout_describe", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_describe(@channel_layout, @buf, @buf_size);
        };
        
        vectors.av_channel_layout_describe_bprint = (AVChannelLayout* @channel_layout, AVBPrint* @bp) =>
        {
            vectors.av_channel_layout_describe_bprint = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_describe_bprint_delegate>("avutil", "av_channel_layout_describe_bprint", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_describe_bprint(@channel_layout, @bp);
        };
        
        vectors.av_channel_layout_extract_channel = (ulong @channel_layout, int @index) =>
        {
            vectors.av_channel_layout_extract_channel = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_extract_channel_delegate>("avutil", "av_channel_layout_extract_channel", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_extract_channel(@channel_layout, @index);
        };
        
        vectors.av_channel_layout_from_mask = (AVChannelLayout* @channel_layout, ulong @mask) =>
        {
            vectors.av_channel_layout_from_mask = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_from_mask_delegate>("avutil", "av_channel_layout_from_mask", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_from_mask(@channel_layout, @mask);
        };
        
        vectors.av_channel_layout_from_string = (AVChannelLayout* @channel_layout, string @str) =>
        {
            vectors.av_channel_layout_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_from_string_delegate>("avutil", "av_channel_layout_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_from_string(@channel_layout, @str);
        };
        
        vectors.av_channel_layout_index_from_channel = (AVChannelLayout* @channel_layout, AVChannel @channel) =>
        {
            vectors.av_channel_layout_index_from_channel = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_index_from_channel_delegate>("avutil", "av_channel_layout_index_from_channel", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_index_from_channel(@channel_layout, @channel);
        };
        
        vectors.av_channel_layout_index_from_string = (AVChannelLayout* @channel_layout, string @name) =>
        {
            vectors.av_channel_layout_index_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_index_from_string_delegate>("avutil", "av_channel_layout_index_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_index_from_string(@channel_layout, @name);
        };
        
        vectors.av_channel_layout_standard = (void** @opaque) =>
        {
            vectors.av_channel_layout_standard = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_standard_delegate>("avutil", "av_channel_layout_standard", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_standard(@opaque);
        };
        
        vectors.av_channel_layout_subset = (AVChannelLayout* @channel_layout, ulong @mask) =>
        {
            vectors.av_channel_layout_subset = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_subset_delegate>("avutil", "av_channel_layout_subset", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_layout_subset(@channel_layout, @mask);
        };
        
        vectors.av_channel_layout_uninit = (AVChannelLayout* @channel_layout) =>
        {
            vectors.av_channel_layout_uninit = FunctionResolver.GetFunctionDelegate<vectors.av_channel_layout_uninit_delegate>("avutil", "av_channel_layout_uninit", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_channel_layout_uninit(@channel_layout);
        };
        
        vectors.av_channel_name = (byte* @buf, ulong @buf_size, AVChannel @channel) =>
        {
            vectors.av_channel_name = FunctionResolver.GetFunctionDelegate<vectors.av_channel_name_delegate>("avutil", "av_channel_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_channel_name(@buf, @buf_size, @channel);
        };
        
        vectors.av_channel_name_bprint = (AVBPrint* @bp, AVChannel @channel_id) =>
        {
            vectors.av_channel_name_bprint = FunctionResolver.GetFunctionDelegate<vectors.av_channel_name_bprint_delegate>("avutil", "av_channel_name_bprint", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_channel_name_bprint(@bp, @channel_id);
        };
        
        vectors.av_chroma_location_from_name = (string @name) =>
        {
            vectors.av_chroma_location_from_name = FunctionResolver.GetFunctionDelegate<vectors.av_chroma_location_from_name_delegate>("avutil", "av_chroma_location_from_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_chroma_location_from_name(@name);
        };
        
        vectors.av_chroma_location_name = (AVChromaLocation @location) =>
        {
            vectors.av_chroma_location_name = FunctionResolver.GetFunctionDelegate<vectors.av_chroma_location_name_delegate>("avutil", "av_chroma_location_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_chroma_location_name(@location);
        };
        
        vectors.av_codec_get_id = (AVCodecTag** @tags, uint @tag) =>
        {
            vectors.av_codec_get_id = FunctionResolver.GetFunctionDelegate<vectors.av_codec_get_id_delegate>("avformat", "av_codec_get_id", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_get_id(@tags, @tag);
        };
        
        vectors.av_codec_get_tag = (AVCodecTag** @tags, AVCodecID @id) =>
        {
            vectors.av_codec_get_tag = FunctionResolver.GetFunctionDelegate<vectors.av_codec_get_tag_delegate>("avformat", "av_codec_get_tag", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_get_tag(@tags, @id);
        };
        
        vectors.av_codec_get_tag2 = (AVCodecTag** @tags, AVCodecID @id, uint* @tag) =>
        {
            vectors.av_codec_get_tag2 = FunctionResolver.GetFunctionDelegate<vectors.av_codec_get_tag2_delegate>("avformat", "av_codec_get_tag2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_get_tag2(@tags, @id, @tag);
        };
        
        vectors.av_codec_is_decoder = (AVCodec* @codec) =>
        {
            vectors.av_codec_is_decoder = FunctionResolver.GetFunctionDelegate<vectors.av_codec_is_decoder_delegate>("avcodec", "av_codec_is_decoder", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_is_decoder(@codec);
        };
        
        vectors.av_codec_is_encoder = (AVCodec* @codec) =>
        {
            vectors.av_codec_is_encoder = FunctionResolver.GetFunctionDelegate<vectors.av_codec_is_encoder_delegate>("avcodec", "av_codec_is_encoder", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_is_encoder(@codec);
        };
        
        vectors.av_codec_iterate = (void** @opaque) =>
        {
            vectors.av_codec_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_codec_iterate_delegate>("avcodec", "av_codec_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_codec_iterate(@opaque);
        };
        
        vectors.av_color_primaries_from_name = (string @name) =>
        {
            vectors.av_color_primaries_from_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_primaries_from_name_delegate>("avutil", "av_color_primaries_from_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_primaries_from_name(@name);
        };
        
        vectors.av_color_primaries_name = (AVColorPrimaries @primaries) =>
        {
            vectors.av_color_primaries_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_primaries_name_delegate>("avutil", "av_color_primaries_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_primaries_name(@primaries);
        };
        
        vectors.av_color_range_from_name = (string @name) =>
        {
            vectors.av_color_range_from_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_range_from_name_delegate>("avutil", "av_color_range_from_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_range_from_name(@name);
        };
        
        vectors.av_color_range_name = (AVColorRange @range) =>
        {
            vectors.av_color_range_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_range_name_delegate>("avutil", "av_color_range_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_range_name(@range);
        };
        
        vectors.av_color_space_from_name = (string @name) =>
        {
            vectors.av_color_space_from_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_space_from_name_delegate>("avutil", "av_color_space_from_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_space_from_name(@name);
        };
        
        vectors.av_color_space_name = (AVColorSpace @space) =>
        {
            vectors.av_color_space_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_space_name_delegate>("avutil", "av_color_space_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_space_name(@space);
        };
        
        vectors.av_color_transfer_from_name = (string @name) =>
        {
            vectors.av_color_transfer_from_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_transfer_from_name_delegate>("avutil", "av_color_transfer_from_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_transfer_from_name(@name);
        };
        
        vectors.av_color_transfer_name = (AVColorTransferCharacteristic @transfer) =>
        {
            vectors.av_color_transfer_name = FunctionResolver.GetFunctionDelegate<vectors.av_color_transfer_name_delegate>("avutil", "av_color_transfer_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_color_transfer_name(@transfer);
        };
        
        vectors.av_compare_mod = (ulong @a, ulong @b, ulong @mod) =>
        {
            vectors.av_compare_mod = FunctionResolver.GetFunctionDelegate<vectors.av_compare_mod_delegate>("avutil", "av_compare_mod", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_compare_mod(@a, @b, @mod);
        };
        
        vectors.av_compare_ts = (long @ts_a, AVRational @tb_a, long @ts_b, AVRational @tb_b) =>
        {
            vectors.av_compare_ts = FunctionResolver.GetFunctionDelegate<vectors.av_compare_ts_delegate>("avutil", "av_compare_ts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_compare_ts(@ts_a, @tb_a, @ts_b, @tb_b);
        };
        
        vectors.av_content_light_metadata_alloc = (ulong* @size) =>
        {
            vectors.av_content_light_metadata_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_content_light_metadata_alloc_delegate>("avutil", "av_content_light_metadata_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_content_light_metadata_alloc(@size);
        };
        
        vectors.av_content_light_metadata_create_side_data = (AVFrame* @frame) =>
        {
            vectors.av_content_light_metadata_create_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_content_light_metadata_create_side_data_delegate>("avutil", "av_content_light_metadata_create_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_content_light_metadata_create_side_data(@frame);
        };
        
        vectors.av_cpb_properties_alloc = (ulong* @size) =>
        {
            vectors.av_cpb_properties_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_cpb_properties_alloc_delegate>("avcodec", "av_cpb_properties_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_cpb_properties_alloc(@size);
        };
        
        vectors.av_cpu_count = () =>
        {
            vectors.av_cpu_count = FunctionResolver.GetFunctionDelegate<vectors.av_cpu_count_delegate>("avutil", "av_cpu_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_cpu_count();
        };
        
        vectors.av_cpu_force_count = (int @count) =>
        {
            vectors.av_cpu_force_count = FunctionResolver.GetFunctionDelegate<vectors.av_cpu_force_count_delegate>("avutil", "av_cpu_force_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_cpu_force_count(@count);
        };
        
        vectors.av_cpu_max_align = () =>
        {
            vectors.av_cpu_max_align = FunctionResolver.GetFunctionDelegate<vectors.av_cpu_max_align_delegate>("avutil", "av_cpu_max_align", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_cpu_max_align();
        };
        
        vectors.av_d2q = (double @d, int @max) =>
        {
            vectors.av_d2q = FunctionResolver.GetFunctionDelegate<vectors.av_d2q_delegate>("avutil", "av_d2q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_d2q(@d, @max);
        };
        
        vectors.av_d3d11va_alloc_context = () =>
        {
            vectors.av_d3d11va_alloc_context = FunctionResolver.GetFunctionDelegate<vectors.av_d3d11va_alloc_context_delegate>("avcodec", "av_d3d11va_alloc_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_d3d11va_alloc_context();
        };
        
        vectors.av_default_get_category = (void* @ptr) =>
        {
            vectors.av_default_get_category = FunctionResolver.GetFunctionDelegate<vectors.av_default_get_category_delegate>("avutil", "av_default_get_category", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_default_get_category(@ptr);
        };
        
        vectors.av_default_item_name = (void* @ctx) =>
        {
            vectors.av_default_item_name = FunctionResolver.GetFunctionDelegate<vectors.av_default_item_name_delegate>("avutil", "av_default_item_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_default_item_name(@ctx);
        };
        
        vectors.av_demuxer_iterate = (void** @opaque) =>
        {
            vectors.av_demuxer_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_demuxer_iterate_delegate>("avformat", "av_demuxer_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_demuxer_iterate(@opaque);
        };
        
        vectors.av_dict_copy = (AVDictionary** @dst, AVDictionary* @src, int @flags) =>
        {
            vectors.av_dict_copy = FunctionResolver.GetFunctionDelegate<vectors.av_dict_copy_delegate>("avutil", "av_dict_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_copy(@dst, @src, @flags);
        };
        
        vectors.av_dict_count = (AVDictionary* @m) =>
        {
            vectors.av_dict_count = FunctionResolver.GetFunctionDelegate<vectors.av_dict_count_delegate>("avutil", "av_dict_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_count(@m);
        };
        
        vectors.av_dict_free = (AVDictionary** @m) =>
        {
            vectors.av_dict_free = FunctionResolver.GetFunctionDelegate<vectors.av_dict_free_delegate>("avutil", "av_dict_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_dict_free(@m);
        };
        
        vectors.av_dict_get = (AVDictionary* @m, string @key, AVDictionaryEntry* @prev, int @flags) =>
        {
            vectors.av_dict_get = FunctionResolver.GetFunctionDelegate<vectors.av_dict_get_delegate>("avutil", "av_dict_get", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_get(@m, @key, @prev, @flags);
        };
        
        vectors.av_dict_get_string = (AVDictionary* @m, byte** @buffer, byte @key_val_sep, byte @pairs_sep) =>
        {
            vectors.av_dict_get_string = FunctionResolver.GetFunctionDelegate<vectors.av_dict_get_string_delegate>("avutil", "av_dict_get_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_get_string(@m, @buffer, @key_val_sep, @pairs_sep);
        };
        
        vectors.av_dict_parse_string = (AVDictionary** @pm, string @str, string @key_val_sep, string @pairs_sep, int @flags) =>
        {
            vectors.av_dict_parse_string = FunctionResolver.GetFunctionDelegate<vectors.av_dict_parse_string_delegate>("avutil", "av_dict_parse_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_parse_string(@pm, @str, @key_val_sep, @pairs_sep, @flags);
        };
        
        vectors.av_dict_set = (AVDictionary** @pm, string @key, string @value, int @flags) =>
        {
            vectors.av_dict_set = FunctionResolver.GetFunctionDelegate<vectors.av_dict_set_delegate>("avutil", "av_dict_set", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_set(@pm, @key, @value, @flags);
        };
        
        vectors.av_dict_set_int = (AVDictionary** @pm, string @key, long @value, int @flags) =>
        {
            vectors.av_dict_set_int = FunctionResolver.GetFunctionDelegate<vectors.av_dict_set_int_delegate>("avutil", "av_dict_set_int", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dict_set_int(@pm, @key, @value, @flags);
        };
        
        vectors.av_disposition_from_string = (string @disp) =>
        {
            vectors.av_disposition_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_disposition_from_string_delegate>("avformat", "av_disposition_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_disposition_from_string(@disp);
        };
        
        vectors.av_disposition_to_string = (int @disposition) =>
        {
            vectors.av_disposition_to_string = FunctionResolver.GetFunctionDelegate<vectors.av_disposition_to_string_delegate>("avformat", "av_disposition_to_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_disposition_to_string(@disposition);
        };
        
        vectors.av_div_q = (AVRational @b, AVRational @c) =>
        {
            vectors.av_div_q = FunctionResolver.GetFunctionDelegate<vectors.av_div_q_delegate>("avutil", "av_div_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_div_q(@b, @c);
        };
        
        vectors.av_dump_format = (AVFormatContext* @ic, int @index, string @url, int @is_output) =>
        {
            vectors.av_dump_format = FunctionResolver.GetFunctionDelegate<vectors.av_dump_format_delegate>("avformat", "av_dump_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_dump_format(@ic, @index, @url, @is_output);
        };
        
        vectors.av_dynamic_hdr_plus_alloc = (ulong* @size) =>
        {
            vectors.av_dynamic_hdr_plus_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_dynamic_hdr_plus_alloc_delegate>("avutil", "av_dynamic_hdr_plus_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dynamic_hdr_plus_alloc(@size);
        };
        
        vectors.av_dynamic_hdr_plus_create_side_data = (AVFrame* @frame) =>
        {
            vectors.av_dynamic_hdr_plus_create_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_dynamic_hdr_plus_create_side_data_delegate>("avutil", "av_dynamic_hdr_plus_create_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dynamic_hdr_plus_create_side_data(@frame);
        };
        
        vectors.av_dynarray_add = (void* @tab_ptr, int* @nb_ptr, void* @elem) =>
        {
            vectors.av_dynarray_add = FunctionResolver.GetFunctionDelegate<vectors.av_dynarray_add_delegate>("avutil", "av_dynarray_add", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_dynarray_add(@tab_ptr, @nb_ptr, @elem);
        };
        
        vectors.av_dynarray_add_nofree = (void* @tab_ptr, int* @nb_ptr, void* @elem) =>
        {
            vectors.av_dynarray_add_nofree = FunctionResolver.GetFunctionDelegate<vectors.av_dynarray_add_nofree_delegate>("avutil", "av_dynarray_add_nofree", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dynarray_add_nofree(@tab_ptr, @nb_ptr, @elem);
        };
        
        vectors.av_dynarray2_add = (void** @tab_ptr, int* @nb_ptr, ulong @elem_size, byte* @elem_data) =>
        {
            vectors.av_dynarray2_add = FunctionResolver.GetFunctionDelegate<vectors.av_dynarray2_add_delegate>("avutil", "av_dynarray2_add", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_dynarray2_add(@tab_ptr, @nb_ptr, @elem_size, @elem_data);
        };
        
        vectors.av_fast_malloc = (void* @ptr, uint* @size, ulong @min_size) =>
        {
            vectors.av_fast_malloc = FunctionResolver.GetFunctionDelegate<vectors.av_fast_malloc_delegate>("avutil", "av_fast_malloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_fast_malloc(@ptr, @size, @min_size);
        };
        
        vectors.av_fast_mallocz = (void* @ptr, uint* @size, ulong @min_size) =>
        {
            vectors.av_fast_mallocz = FunctionResolver.GetFunctionDelegate<vectors.av_fast_mallocz_delegate>("avutil", "av_fast_mallocz", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_fast_mallocz(@ptr, @size, @min_size);
        };
        
        vectors.av_fast_padded_malloc = (void* @ptr, uint* @size, ulong @min_size) =>
        {
            vectors.av_fast_padded_malloc = FunctionResolver.GetFunctionDelegate<vectors.av_fast_padded_malloc_delegate>("avcodec", "av_fast_padded_malloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_fast_padded_malloc(@ptr, @size, @min_size);
        };
        
        vectors.av_fast_padded_mallocz = (void* @ptr, uint* @size, ulong @min_size) =>
        {
            vectors.av_fast_padded_mallocz = FunctionResolver.GetFunctionDelegate<vectors.av_fast_padded_mallocz_delegate>("avcodec", "av_fast_padded_mallocz", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_fast_padded_mallocz(@ptr, @size, @min_size);
        };
        
        vectors.av_fast_realloc = (void* @ptr, uint* @size, ulong @min_size) =>
        {
            vectors.av_fast_realloc = FunctionResolver.GetFunctionDelegate<vectors.av_fast_realloc_delegate>("avutil", "av_fast_realloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_fast_realloc(@ptr, @size, @min_size);
        };
        
        vectors.av_file_map = (string @filename, byte** @bufptr, ulong* @size, int @log_offset, void* @log_ctx) =>
        {
            vectors.av_file_map = FunctionResolver.GetFunctionDelegate<vectors.av_file_map_delegate>("avutil", "av_file_map", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_file_map(@filename, @bufptr, @size, @log_offset, @log_ctx);
        };
        
        vectors.av_file_unmap = (byte* @bufptr, ulong @size) =>
        {
            vectors.av_file_unmap = FunctionResolver.GetFunctionDelegate<vectors.av_file_unmap_delegate>("avutil", "av_file_unmap", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_file_unmap(@bufptr, @size);
        };
        
        vectors.av_filename_number_test = (string @filename) =>
        {
            vectors.av_filename_number_test = FunctionResolver.GetFunctionDelegate<vectors.av_filename_number_test_delegate>("avformat", "av_filename_number_test", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_filename_number_test(@filename);
        };
        
        vectors.av_filter_iterate = (void** @opaque) =>
        {
            vectors.av_filter_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_filter_iterate_delegate>("avfilter", "av_filter_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_filter_iterate(@opaque);
        };
        
        vectors.av_find_best_pix_fmt_of_2 = (AVPixelFormat @dst_pix_fmt1, AVPixelFormat @dst_pix_fmt2, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr) =>
        {
            vectors.av_find_best_pix_fmt_of_2 = FunctionResolver.GetFunctionDelegate<vectors.av_find_best_pix_fmt_of_2_delegate>("avutil", "av_find_best_pix_fmt_of_2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_best_pix_fmt_of_2(@dst_pix_fmt1, @dst_pix_fmt2, @src_pix_fmt, @has_alpha, @loss_ptr);
        };
        
        vectors.av_find_best_stream = (AVFormatContext* @ic, AVMediaType @type, int @wanted_stream_nb, int @related_stream, AVCodec** @decoder_ret, int @flags) =>
        {
            vectors.av_find_best_stream = FunctionResolver.GetFunctionDelegate<vectors.av_find_best_stream_delegate>("avformat", "av_find_best_stream", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_best_stream(@ic, @type, @wanted_stream_nb, @related_stream, @decoder_ret, @flags);
        };
        
        vectors.av_find_default_stream_index = (AVFormatContext* @s) =>
        {
            vectors.av_find_default_stream_index = FunctionResolver.GetFunctionDelegate<vectors.av_find_default_stream_index_delegate>("avformat", "av_find_default_stream_index", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_default_stream_index(@s);
        };
        
        vectors.av_find_input_format = (string @short_name) =>
        {
            vectors.av_find_input_format = FunctionResolver.GetFunctionDelegate<vectors.av_find_input_format_delegate>("avformat", "av_find_input_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_input_format(@short_name);
        };
        
        vectors.av_find_nearest_q_idx = (AVRational @q, AVRational* @q_list) =>
        {
            vectors.av_find_nearest_q_idx = FunctionResolver.GetFunctionDelegate<vectors.av_find_nearest_q_idx_delegate>("avutil", "av_find_nearest_q_idx", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_nearest_q_idx(@q, @q_list);
        };
        
        vectors.av_find_program_from_stream = (AVFormatContext* @ic, AVProgram* @last, int @s) =>
        {
            vectors.av_find_program_from_stream = FunctionResolver.GetFunctionDelegate<vectors.av_find_program_from_stream_delegate>("avformat", "av_find_program_from_stream", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_find_program_from_stream(@ic, @last, @s);
        };
        
        vectors.av_fmt_ctx_get_duration_estimation_method = (AVFormatContext* @ctx) =>
        {
            vectors.av_fmt_ctx_get_duration_estimation_method = FunctionResolver.GetFunctionDelegate<vectors.av_fmt_ctx_get_duration_estimation_method_delegate>("avformat", "av_fmt_ctx_get_duration_estimation_method", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_fmt_ctx_get_duration_estimation_method(@ctx);
        };
        
        vectors.av_fopen_utf8 = (string @path, string @mode) =>
        {
            vectors.av_fopen_utf8 = FunctionResolver.GetFunctionDelegate<vectors.av_fopen_utf8_delegate>("avutil", "av_fopen_utf8", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_fopen_utf8(@path, @mode);
        };
        
        vectors.av_force_cpu_flags = (int @flags) =>
        {
            vectors.av_force_cpu_flags = FunctionResolver.GetFunctionDelegate<vectors.av_force_cpu_flags_delegate>("avutil", "av_force_cpu_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_force_cpu_flags(@flags);
        };
        
        vectors.av_format_inject_global_side_data = (AVFormatContext* @s) =>
        {
            vectors.av_format_inject_global_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_format_inject_global_side_data_delegate>("avformat", "av_format_inject_global_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_format_inject_global_side_data(@s);
        };
        
        vectors.av_fourcc_make_string = (byte* @buf, uint @fourcc) =>
        {
            vectors.av_fourcc_make_string = FunctionResolver.GetFunctionDelegate<vectors.av_fourcc_make_string_delegate>("avutil", "av_fourcc_make_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_fourcc_make_string(@buf, @fourcc);
        };
        
        vectors.av_frame_alloc = () =>
        {
            vectors.av_frame_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_frame_alloc_delegate>("avutil", "av_frame_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_alloc();
        };
        
        vectors.av_frame_apply_cropping = (AVFrame* @frame, int @flags) =>
        {
            vectors.av_frame_apply_cropping = FunctionResolver.GetFunctionDelegate<vectors.av_frame_apply_cropping_delegate>("avutil", "av_frame_apply_cropping", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_apply_cropping(@frame, @flags);
        };
        
        vectors.av_frame_clone = (AVFrame* @src) =>
        {
            vectors.av_frame_clone = FunctionResolver.GetFunctionDelegate<vectors.av_frame_clone_delegate>("avutil", "av_frame_clone", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_clone(@src);
        };
        
        vectors.av_frame_copy = (AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.av_frame_copy = FunctionResolver.GetFunctionDelegate<vectors.av_frame_copy_delegate>("avutil", "av_frame_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_copy(@dst, @src);
        };
        
        vectors.av_frame_copy_props = (AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.av_frame_copy_props = FunctionResolver.GetFunctionDelegate<vectors.av_frame_copy_props_delegate>("avutil", "av_frame_copy_props", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_copy_props(@dst, @src);
        };
        
        vectors.av_frame_free = (AVFrame** @frame) =>
        {
            vectors.av_frame_free = FunctionResolver.GetFunctionDelegate<vectors.av_frame_free_delegate>("avutil", "av_frame_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_frame_free(@frame);
        };
        
        vectors.av_frame_get_buffer = (AVFrame* @frame, int @align) =>
        {
            vectors.av_frame_get_buffer = FunctionResolver.GetFunctionDelegate<vectors.av_frame_get_buffer_delegate>("avutil", "av_frame_get_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_get_buffer(@frame, @align);
        };
        
        vectors.av_frame_get_plane_buffer = (AVFrame* @frame, int @plane) =>
        {
            vectors.av_frame_get_plane_buffer = FunctionResolver.GetFunctionDelegate<vectors.av_frame_get_plane_buffer_delegate>("avutil", "av_frame_get_plane_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_get_plane_buffer(@frame, @plane);
        };
        
        vectors.av_frame_get_side_data = (AVFrame* @frame, AVFrameSideDataType @type) =>
        {
            vectors.av_frame_get_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_frame_get_side_data_delegate>("avutil", "av_frame_get_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_get_side_data(@frame, @type);
        };
        
        vectors.av_frame_is_writable = (AVFrame* @frame) =>
        {
            vectors.av_frame_is_writable = FunctionResolver.GetFunctionDelegate<vectors.av_frame_is_writable_delegate>("avutil", "av_frame_is_writable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_is_writable(@frame);
        };
        
        vectors.av_frame_make_writable = (AVFrame* @frame) =>
        {
            vectors.av_frame_make_writable = FunctionResolver.GetFunctionDelegate<vectors.av_frame_make_writable_delegate>("avutil", "av_frame_make_writable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_make_writable(@frame);
        };
        
        vectors.av_frame_move_ref = (AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.av_frame_move_ref = FunctionResolver.GetFunctionDelegate<vectors.av_frame_move_ref_delegate>("avutil", "av_frame_move_ref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_frame_move_ref(@dst, @src);
        };
        
        vectors.av_frame_new_side_data = (AVFrame* @frame, AVFrameSideDataType @type, ulong @size) =>
        {
            vectors.av_frame_new_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_frame_new_side_data_delegate>("avutil", "av_frame_new_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_new_side_data(@frame, @type, @size);
        };
        
        vectors.av_frame_new_side_data_from_buf = (AVFrame* @frame, AVFrameSideDataType @type, AVBufferRef* @buf) =>
        {
            vectors.av_frame_new_side_data_from_buf = FunctionResolver.GetFunctionDelegate<vectors.av_frame_new_side_data_from_buf_delegate>("avutil", "av_frame_new_side_data_from_buf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_new_side_data_from_buf(@frame, @type, @buf);
        };
        
        vectors.av_frame_ref = (AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.av_frame_ref = FunctionResolver.GetFunctionDelegate<vectors.av_frame_ref_delegate>("avutil", "av_frame_ref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_ref(@dst, @src);
        };
        
        vectors.av_frame_remove_side_data = (AVFrame* @frame, AVFrameSideDataType @type) =>
        {
            vectors.av_frame_remove_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_frame_remove_side_data_delegate>("avutil", "av_frame_remove_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_frame_remove_side_data(@frame, @type);
        };
        
        vectors.av_frame_side_data_name = (AVFrameSideDataType @type) =>
        {
            vectors.av_frame_side_data_name = FunctionResolver.GetFunctionDelegate<vectors.av_frame_side_data_name_delegate>("avutil", "av_frame_side_data_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_frame_side_data_name(@type);
        };
        
        vectors.av_frame_unref = (AVFrame* @frame) =>
        {
            vectors.av_frame_unref = FunctionResolver.GetFunctionDelegate<vectors.av_frame_unref_delegate>("avutil", "av_frame_unref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_frame_unref(@frame);
        };
        
        vectors.av_free = (void* @ptr) =>
        {
            vectors.av_free = FunctionResolver.GetFunctionDelegate<vectors.av_free_delegate>("avutil", "av_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_free(@ptr);
        };
        
        vectors.av_freep = (void* @ptr) =>
        {
            vectors.av_freep = FunctionResolver.GetFunctionDelegate<vectors.av_freep_delegate>("avutil", "av_freep", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_freep(@ptr);
        };
        
        vectors.av_gcd = (long @a, long @b) =>
        {
            vectors.av_gcd = FunctionResolver.GetFunctionDelegate<vectors.av_gcd_delegate>("avutil", "av_gcd", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_gcd(@a, @b);
        };
        
        vectors.av_gcd_q = (AVRational @a, AVRational @b, int @max_den, AVRational @def) =>
        {
            vectors.av_gcd_q = FunctionResolver.GetFunctionDelegate<vectors.av_gcd_q_delegate>("avutil", "av_gcd_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_gcd_q(@a, @b, @max_den, @def);
        };
        
        vectors.av_get_alt_sample_fmt = (AVSampleFormat @sample_fmt, int @planar) =>
        {
            vectors.av_get_alt_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_get_alt_sample_fmt_delegate>("avutil", "av_get_alt_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_alt_sample_fmt(@sample_fmt, @planar);
        };
        
        vectors.av_get_audio_frame_duration = (AVCodecContext* @avctx, int @frame_bytes) =>
        {
            vectors.av_get_audio_frame_duration = FunctionResolver.GetFunctionDelegate<vectors.av_get_audio_frame_duration_delegate>("avcodec", "av_get_audio_frame_duration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_audio_frame_duration(@avctx, @frame_bytes);
        };
        
        vectors.av_get_audio_frame_duration2 = (AVCodecParameters* @par, int @frame_bytes) =>
        {
            vectors.av_get_audio_frame_duration2 = FunctionResolver.GetFunctionDelegate<vectors.av_get_audio_frame_duration2_delegate>("avcodec", "av_get_audio_frame_duration2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_audio_frame_duration2(@par, @frame_bytes);
        };
        
        vectors.av_get_bits_per_pixel = (AVPixFmtDescriptor* @pixdesc) =>
        {
            vectors.av_get_bits_per_pixel = FunctionResolver.GetFunctionDelegate<vectors.av_get_bits_per_pixel_delegate>("avutil", "av_get_bits_per_pixel", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_bits_per_pixel(@pixdesc);
        };
        
        vectors.av_get_bits_per_sample = (AVCodecID @codec_id) =>
        {
            vectors.av_get_bits_per_sample = FunctionResolver.GetFunctionDelegate<vectors.av_get_bits_per_sample_delegate>("avcodec", "av_get_bits_per_sample", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_bits_per_sample(@codec_id);
        };
        
        vectors.av_get_bytes_per_sample = (AVSampleFormat @sample_fmt) =>
        {
            vectors.av_get_bytes_per_sample = FunctionResolver.GetFunctionDelegate<vectors.av_get_bytes_per_sample_delegate>("avutil", "av_get_bytes_per_sample", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_bytes_per_sample(@sample_fmt);
        };
        
        vectors.av_get_channel_description = (ulong @channel) =>
        {
            vectors.av_get_channel_description = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_description_delegate>("avutil", "av_get_channel_description", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_channel_description(@channel);
        };
        
        vectors.av_get_channel_layout = (string @name) =>
        {
            vectors.av_get_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_layout_delegate>("avutil", "av_get_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_channel_layout(@name);
        };
        
        vectors.av_get_channel_layout_channel_index = (ulong @channel_layout, ulong @channel) =>
        {
            vectors.av_get_channel_layout_channel_index = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_layout_channel_index_delegate>("avutil", "av_get_channel_layout_channel_index", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_channel_layout_channel_index(@channel_layout, @channel);
        };
        
        vectors.av_get_channel_layout_nb_channels = (ulong @channel_layout) =>
        {
            vectors.av_get_channel_layout_nb_channels = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_layout_nb_channels_delegate>("avutil", "av_get_channel_layout_nb_channels", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_channel_layout_nb_channels(@channel_layout);
        };
        
        vectors.av_get_channel_layout_string = (byte* @buf, int @buf_size, int @nb_channels, ulong @channel_layout) =>
        {
            vectors.av_get_channel_layout_string = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_layout_string_delegate>("avutil", "av_get_channel_layout_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_get_channel_layout_string(@buf, @buf_size, @nb_channels, @channel_layout);
        };
        
        vectors.av_get_channel_name = (ulong @channel) =>
        {
            vectors.av_get_channel_name = FunctionResolver.GetFunctionDelegate<vectors.av_get_channel_name_delegate>("avutil", "av_get_channel_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_channel_name(@channel);
        };
        
        vectors.av_get_colorspace_name = (AVColorSpace @val) =>
        {
            vectors.av_get_colorspace_name = FunctionResolver.GetFunctionDelegate<vectors.av_get_colorspace_name_delegate>("avutil", "av_get_colorspace_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_colorspace_name(@val);
        };
        
        vectors.av_get_cpu_flags = () =>
        {
            vectors.av_get_cpu_flags = FunctionResolver.GetFunctionDelegate<vectors.av_get_cpu_flags_delegate>("avutil", "av_get_cpu_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_cpu_flags();
        };
        
        vectors.av_get_default_channel_layout = (int @nb_channels) =>
        {
            vectors.av_get_default_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_get_default_channel_layout_delegate>("avutil", "av_get_default_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_default_channel_layout(@nb_channels);
        };
        
        vectors.av_get_exact_bits_per_sample = (AVCodecID @codec_id) =>
        {
            vectors.av_get_exact_bits_per_sample = FunctionResolver.GetFunctionDelegate<vectors.av_get_exact_bits_per_sample_delegate>("avcodec", "av_get_exact_bits_per_sample", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_exact_bits_per_sample(@codec_id);
        };
        
        vectors.av_get_extended_channel_layout = (string @name, ulong* @channel_layout, int* @nb_channels) =>
        {
            vectors.av_get_extended_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_get_extended_channel_layout_delegate>("avutil", "av_get_extended_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_extended_channel_layout(@name, @channel_layout, @nb_channels);
        };
        
        vectors.av_get_frame_filename = (byte* @buf, int @buf_size, string @path, int @number) =>
        {
            vectors.av_get_frame_filename = FunctionResolver.GetFunctionDelegate<vectors.av_get_frame_filename_delegate>("avformat", "av_get_frame_filename", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_frame_filename(@buf, @buf_size, @path, @number);
        };
        
        vectors.av_get_frame_filename2 = (byte* @buf, int @buf_size, string @path, int @number, int @flags) =>
        {
            vectors.av_get_frame_filename2 = FunctionResolver.GetFunctionDelegate<vectors.av_get_frame_filename2_delegate>("avformat", "av_get_frame_filename2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_frame_filename2(@buf, @buf_size, @path, @number, @flags);
        };
        
        vectors.av_get_media_type_string = (AVMediaType @media_type) =>
        {
            vectors.av_get_media_type_string = FunctionResolver.GetFunctionDelegate<vectors.av_get_media_type_string_delegate>("avutil", "av_get_media_type_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_media_type_string(@media_type);
        };
        
        vectors.av_get_output_timestamp = (AVFormatContext* @s, int @stream, long* @dts, long* @wall) =>
        {
            vectors.av_get_output_timestamp = FunctionResolver.GetFunctionDelegate<vectors.av_get_output_timestamp_delegate>("avformat", "av_get_output_timestamp", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_output_timestamp(@s, @stream, @dts, @wall);
        };
        
        vectors.av_get_packed_sample_fmt = (AVSampleFormat @sample_fmt) =>
        {
            vectors.av_get_packed_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_get_packed_sample_fmt_delegate>("avutil", "av_get_packed_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_packed_sample_fmt(@sample_fmt);
        };
        
        vectors.av_get_packet = (AVIOContext* @s, AVPacket* @pkt, int @size) =>
        {
            vectors.av_get_packet = FunctionResolver.GetFunctionDelegate<vectors.av_get_packet_delegate>("avformat", "av_get_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_packet(@s, @pkt, @size);
        };
        
        vectors.av_get_padded_bits_per_pixel = (AVPixFmtDescriptor* @pixdesc) =>
        {
            vectors.av_get_padded_bits_per_pixel = FunctionResolver.GetFunctionDelegate<vectors.av_get_padded_bits_per_pixel_delegate>("avutil", "av_get_padded_bits_per_pixel", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_padded_bits_per_pixel(@pixdesc);
        };
        
        vectors.av_get_pcm_codec = (AVSampleFormat @fmt, int @be) =>
        {
            vectors.av_get_pcm_codec = FunctionResolver.GetFunctionDelegate<vectors.av_get_pcm_codec_delegate>("avcodec", "av_get_pcm_codec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_pcm_codec(@fmt, @be);
        };
        
        vectors.av_get_picture_type_char = (AVPictureType @pict_type) =>
        {
            vectors.av_get_picture_type_char = FunctionResolver.GetFunctionDelegate<vectors.av_get_picture_type_char_delegate>("avutil", "av_get_picture_type_char", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_picture_type_char(@pict_type);
        };
        
        vectors.av_get_pix_fmt = (string @name) =>
        {
            vectors.av_get_pix_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_get_pix_fmt_delegate>("avutil", "av_get_pix_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_pix_fmt(@name);
        };
        
        vectors.av_get_pix_fmt_loss = (AVPixelFormat @dst_pix_fmt, AVPixelFormat @src_pix_fmt, int @has_alpha) =>
        {
            vectors.av_get_pix_fmt_loss = FunctionResolver.GetFunctionDelegate<vectors.av_get_pix_fmt_loss_delegate>("avutil", "av_get_pix_fmt_loss", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_pix_fmt_loss(@dst_pix_fmt, @src_pix_fmt, @has_alpha);
        };
        
        vectors.av_get_pix_fmt_name = (AVPixelFormat @pix_fmt) =>
        {
            vectors.av_get_pix_fmt_name = FunctionResolver.GetFunctionDelegate<vectors.av_get_pix_fmt_name_delegate>("avutil", "av_get_pix_fmt_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_pix_fmt_name(@pix_fmt);
        };
        
        vectors.av_get_pix_fmt_string = (byte* @buf, int @buf_size, AVPixelFormat @pix_fmt) =>
        {
            vectors.av_get_pix_fmt_string = FunctionResolver.GetFunctionDelegate<vectors.av_get_pix_fmt_string_delegate>("avutil", "av_get_pix_fmt_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_pix_fmt_string(@buf, @buf_size, @pix_fmt);
        };
        
        vectors.av_get_planar_sample_fmt = (AVSampleFormat @sample_fmt) =>
        {
            vectors.av_get_planar_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_get_planar_sample_fmt_delegate>("avutil", "av_get_planar_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_planar_sample_fmt(@sample_fmt);
        };
        
        vectors.av_get_profile_name = (AVCodec* @codec, int @profile) =>
        {
            vectors.av_get_profile_name = FunctionResolver.GetFunctionDelegate<vectors.av_get_profile_name_delegate>("avcodec", "av_get_profile_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_profile_name(@codec, @profile);
        };
        
        vectors.av_get_sample_fmt = (string @name) =>
        {
            vectors.av_get_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_get_sample_fmt_delegate>("avutil", "av_get_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_sample_fmt(@name);
        };
        
        vectors.av_get_sample_fmt_name = (AVSampleFormat @sample_fmt) =>
        {
            vectors.av_get_sample_fmt_name = FunctionResolver.GetFunctionDelegate<vectors.av_get_sample_fmt_name_delegate>("avutil", "av_get_sample_fmt_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_sample_fmt_name(@sample_fmt);
        };
        
        vectors.av_get_sample_fmt_string = (byte* @buf, int @buf_size, AVSampleFormat @sample_fmt) =>
        {
            vectors.av_get_sample_fmt_string = FunctionResolver.GetFunctionDelegate<vectors.av_get_sample_fmt_string_delegate>("avutil", "av_get_sample_fmt_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_sample_fmt_string(@buf, @buf_size, @sample_fmt);
        };
        
        vectors.av_get_standard_channel_layout = (uint @index, ulong* @layout, byte** @name) =>
        {
            vectors.av_get_standard_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_get_standard_channel_layout_delegate>("avutil", "av_get_standard_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_standard_channel_layout(@index, @layout, @name);
        };
        
        vectors.av_get_time_base_q = () =>
        {
            vectors.av_get_time_base_q = FunctionResolver.GetFunctionDelegate<vectors.av_get_time_base_q_delegate>("avutil", "av_get_time_base_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_get_time_base_q();
        };
        
        vectors.av_gettime = () =>
        {
            vectors.av_gettime = FunctionResolver.GetFunctionDelegate<vectors.av_gettime_delegate>("avutil", "av_gettime", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_gettime();
        };
        
        vectors.av_gettime_relative = () =>
        {
            vectors.av_gettime_relative = FunctionResolver.GetFunctionDelegate<vectors.av_gettime_relative_delegate>("avutil", "av_gettime_relative", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_gettime_relative();
        };
        
        vectors.av_gettime_relative_is_monotonic = () =>
        {
            vectors.av_gettime_relative_is_monotonic = FunctionResolver.GetFunctionDelegate<vectors.av_gettime_relative_is_monotonic_delegate>("avutil", "av_gettime_relative_is_monotonic", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_gettime_relative_is_monotonic();
        };
        
        vectors.av_grow_packet = (AVPacket* @pkt, int @grow_by) =>
        {
            vectors.av_grow_packet = FunctionResolver.GetFunctionDelegate<vectors.av_grow_packet_delegate>("avcodec", "av_grow_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_grow_packet(@pkt, @grow_by);
        };
        
        vectors.av_guess_codec = (AVOutputFormat* @fmt, string @short_name, string @filename, string @mime_type, AVMediaType @type) =>
        {
            vectors.av_guess_codec = FunctionResolver.GetFunctionDelegate<vectors.av_guess_codec_delegate>("avformat", "av_guess_codec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_guess_codec(@fmt, @short_name, @filename, @mime_type, @type);
        };
        
        vectors.av_guess_format = (string @short_name, string @filename, string @mime_type) =>
        {
            vectors.av_guess_format = FunctionResolver.GetFunctionDelegate<vectors.av_guess_format_delegate>("avformat", "av_guess_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_guess_format(@short_name, @filename, @mime_type);
        };
        
        vectors.av_guess_frame_rate = (AVFormatContext* @ctx, AVStream* @stream, AVFrame* @frame) =>
        {
            vectors.av_guess_frame_rate = FunctionResolver.GetFunctionDelegate<vectors.av_guess_frame_rate_delegate>("avformat", "av_guess_frame_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_guess_frame_rate(@ctx, @stream, @frame);
        };
        
        vectors.av_guess_sample_aspect_ratio = (AVFormatContext* @format, AVStream* @stream, AVFrame* @frame) =>
        {
            vectors.av_guess_sample_aspect_ratio = FunctionResolver.GetFunctionDelegate<vectors.av_guess_sample_aspect_ratio_delegate>("avformat", "av_guess_sample_aspect_ratio", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_guess_sample_aspect_ratio(@format, @stream, @frame);
        };
        
        vectors.av_hex_dump = (_iobuf* @f, byte* @buf, int @size) =>
        {
            vectors.av_hex_dump = FunctionResolver.GetFunctionDelegate<vectors.av_hex_dump_delegate>("avformat", "av_hex_dump", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_hex_dump(@f, @buf, @size);
        };
        
        vectors.av_hex_dump_log = (void* @avcl, int @level, byte* @buf, int @size) =>
        {
            vectors.av_hex_dump_log = FunctionResolver.GetFunctionDelegate<vectors.av_hex_dump_log_delegate>("avformat", "av_hex_dump_log", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_hex_dump_log(@avcl, @level, @buf, @size);
        };
        
        vectors.av_hwdevice_ctx_alloc = (AVHWDeviceType @type) =>
        {
            vectors.av_hwdevice_ctx_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_ctx_alloc_delegate>("avutil", "av_hwdevice_ctx_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_ctx_alloc(@type);
        };
        
        vectors.av_hwdevice_ctx_create = (AVBufferRef** @device_ctx, AVHWDeviceType @type, string @device, AVDictionary* @opts, int @flags) =>
        {
            vectors.av_hwdevice_ctx_create = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_ctx_create_delegate>("avutil", "av_hwdevice_ctx_create", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_ctx_create(@device_ctx, @type, @device, @opts, @flags);
        };
        
        vectors.av_hwdevice_ctx_create_derived = (AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, int @flags) =>
        {
            vectors.av_hwdevice_ctx_create_derived = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_ctx_create_derived_delegate>("avutil", "av_hwdevice_ctx_create_derived", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_ctx_create_derived(@dst_ctx, @type, @src_ctx, @flags);
        };
        
        vectors.av_hwdevice_ctx_create_derived_opts = (AVBufferRef** @dst_ctx, AVHWDeviceType @type, AVBufferRef* @src_ctx, AVDictionary* @options, int @flags) =>
        {
            vectors.av_hwdevice_ctx_create_derived_opts = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_ctx_create_derived_opts_delegate>("avutil", "av_hwdevice_ctx_create_derived_opts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_ctx_create_derived_opts(@dst_ctx, @type, @src_ctx, @options, @flags);
        };
        
        vectors.av_hwdevice_ctx_init = (AVBufferRef* @ref) =>
        {
            vectors.av_hwdevice_ctx_init = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_ctx_init_delegate>("avutil", "av_hwdevice_ctx_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_ctx_init(@ref);
        };
        
        vectors.av_hwdevice_find_type_by_name = (string @name) =>
        {
            vectors.av_hwdevice_find_type_by_name = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_find_type_by_name_delegate>("avutil", "av_hwdevice_find_type_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_find_type_by_name(@name);
        };
        
        vectors.av_hwdevice_get_hwframe_constraints = (AVBufferRef* @ref, void* @hwconfig) =>
        {
            vectors.av_hwdevice_get_hwframe_constraints = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_get_hwframe_constraints_delegate>("avutil", "av_hwdevice_get_hwframe_constraints", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_get_hwframe_constraints(@ref, @hwconfig);
        };
        
        vectors.av_hwdevice_get_type_name = (AVHWDeviceType @type) =>
        {
            vectors.av_hwdevice_get_type_name = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_get_type_name_delegate>("avutil", "av_hwdevice_get_type_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_get_type_name(@type);
        };
        
        vectors.av_hwdevice_hwconfig_alloc = (AVBufferRef* @device_ctx) =>
        {
            vectors.av_hwdevice_hwconfig_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_hwconfig_alloc_delegate>("avutil", "av_hwdevice_hwconfig_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_hwconfig_alloc(@device_ctx);
        };
        
        vectors.av_hwdevice_iterate_types = (AVHWDeviceType @prev) =>
        {
            vectors.av_hwdevice_iterate_types = FunctionResolver.GetFunctionDelegate<vectors.av_hwdevice_iterate_types_delegate>("avutil", "av_hwdevice_iterate_types", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwdevice_iterate_types(@prev);
        };
        
        vectors.av_hwframe_constraints_free = (AVHWFramesConstraints** @constraints) =>
        {
            vectors.av_hwframe_constraints_free = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_constraints_free_delegate>("avutil", "av_hwframe_constraints_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_hwframe_constraints_free(@constraints);
        };
        
        vectors.av_hwframe_ctx_alloc = (AVBufferRef* @device_ctx) =>
        {
            vectors.av_hwframe_ctx_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_ctx_alloc_delegate>("avutil", "av_hwframe_ctx_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_ctx_alloc(@device_ctx);
        };
        
        vectors.av_hwframe_ctx_create_derived = (AVBufferRef** @derived_frame_ctx, AVPixelFormat @format, AVBufferRef* @derived_device_ctx, AVBufferRef* @source_frame_ctx, int @flags) =>
        {
            vectors.av_hwframe_ctx_create_derived = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_ctx_create_derived_delegate>("avutil", "av_hwframe_ctx_create_derived", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_ctx_create_derived(@derived_frame_ctx, @format, @derived_device_ctx, @source_frame_ctx, @flags);
        };
        
        vectors.av_hwframe_ctx_init = (AVBufferRef* @ref) =>
        {
            vectors.av_hwframe_ctx_init = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_ctx_init_delegate>("avutil", "av_hwframe_ctx_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_ctx_init(@ref);
        };
        
        vectors.av_hwframe_get_buffer = (AVBufferRef* @hwframe_ctx, AVFrame* @frame, int @flags) =>
        {
            vectors.av_hwframe_get_buffer = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_get_buffer_delegate>("avutil", "av_hwframe_get_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_get_buffer(@hwframe_ctx, @frame, @flags);
        };
        
        vectors.av_hwframe_map = (AVFrame* @dst, AVFrame* @src, int @flags) =>
        {
            vectors.av_hwframe_map = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_map_delegate>("avutil", "av_hwframe_map", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_map(@dst, @src, @flags);
        };
        
        vectors.av_hwframe_transfer_data = (AVFrame* @dst, AVFrame* @src, int @flags) =>
        {
            vectors.av_hwframe_transfer_data = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_transfer_data_delegate>("avutil", "av_hwframe_transfer_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_transfer_data(@dst, @src, @flags);
        };
        
        vectors.av_hwframe_transfer_get_formats = (AVBufferRef* @hwframe_ctx, AVHWFrameTransferDirection @dir, AVPixelFormat** @formats, int @flags) =>
        {
            vectors.av_hwframe_transfer_get_formats = FunctionResolver.GetFunctionDelegate<vectors.av_hwframe_transfer_get_formats_delegate>("avutil", "av_hwframe_transfer_get_formats", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_hwframe_transfer_get_formats(@hwframe_ctx, @dir, @formats, @flags);
        };
        
        vectors.av_image_alloc = (ref byte_ptrArray4 @pointers, ref int_array4 @linesizes, int @w, int @h, AVPixelFormat @pix_fmt, int @align) =>
        {
            vectors.av_image_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_image_alloc_delegate>("avutil", "av_image_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_alloc(ref @pointers, ref @linesizes, @w, @h, @pix_fmt, @align);
        };
        
        vectors.av_image_check_sar = (uint @w, uint @h, AVRational @sar) =>
        {
            vectors.av_image_check_sar = FunctionResolver.GetFunctionDelegate<vectors.av_image_check_sar_delegate>("avutil", "av_image_check_sar", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_check_sar(@w, @h, @sar);
        };
        
        vectors.av_image_check_size = (uint @w, uint @h, int @log_offset, void* @log_ctx) =>
        {
            vectors.av_image_check_size = FunctionResolver.GetFunctionDelegate<vectors.av_image_check_size_delegate>("avutil", "av_image_check_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_check_size(@w, @h, @log_offset, @log_ctx);
        };
        
        vectors.av_image_check_size2 = (uint @w, uint @h, long @max_pixels, AVPixelFormat @pix_fmt, int @log_offset, void* @log_ctx) =>
        {
            vectors.av_image_check_size2 = FunctionResolver.GetFunctionDelegate<vectors.av_image_check_size2_delegate>("avutil", "av_image_check_size2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_check_size2(@w, @h, @max_pixels, @pix_fmt, @log_offset, @log_ctx);
        };
        
        vectors.av_image_copy = (ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in int_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height) =>
        {
            vectors.av_image_copy = FunctionResolver.GetFunctionDelegate<vectors.av_image_copy_delegate>("avutil", "av_image_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_image_copy(ref @dst_data, ref @dst_linesizes, @src_data, @src_linesizes, @pix_fmt, @width, @height);
        };
        
        vectors.av_image_copy_plane = (byte* @dst, int @dst_linesize, byte* @src, int @src_linesize, int @bytewidth, int @height) =>
        {
            vectors.av_image_copy_plane = FunctionResolver.GetFunctionDelegate<vectors.av_image_copy_plane_delegate>("avutil", "av_image_copy_plane", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_image_copy_plane(@dst, @dst_linesize, @src, @src_linesize, @bytewidth, @height);
        };
        
        vectors.av_image_copy_plane_uc_from = (byte* @dst, long @dst_linesize, byte* @src, long @src_linesize, long @bytewidth, int @height) =>
        {
            vectors.av_image_copy_plane_uc_from = FunctionResolver.GetFunctionDelegate<vectors.av_image_copy_plane_uc_from_delegate>("avutil", "av_image_copy_plane_uc_from", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_image_copy_plane_uc_from(@dst, @dst_linesize, @src, @src_linesize, @bytewidth, @height);
        };
        
        vectors.av_image_copy_to_buffer = (byte* @dst, int @dst_size, in byte_ptrArray4 @src_data, in int_array4 @src_linesize, AVPixelFormat @pix_fmt, int @width, int @height, int @align) =>
        {
            vectors.av_image_copy_to_buffer = FunctionResolver.GetFunctionDelegate<vectors.av_image_copy_to_buffer_delegate>("avutil", "av_image_copy_to_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_copy_to_buffer(@dst, @dst_size, @src_data, @src_linesize, @pix_fmt, @width, @height, @align);
        };
        
        vectors.av_image_copy_uc_from = (ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesizes, in byte_ptrArray4 @src_data, in long_array4 @src_linesizes, AVPixelFormat @pix_fmt, int @width, int @height) =>
        {
            vectors.av_image_copy_uc_from = FunctionResolver.GetFunctionDelegate<vectors.av_image_copy_uc_from_delegate>("avutil", "av_image_copy_uc_from", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_image_copy_uc_from(ref @dst_data, @dst_linesizes, @src_data, @src_linesizes, @pix_fmt, @width, @height);
        };
        
        vectors.av_image_fill_arrays = (ref byte_ptrArray4 @dst_data, ref int_array4 @dst_linesize, byte* @src, AVPixelFormat @pix_fmt, int @width, int @height, int @align) =>
        {
            vectors.av_image_fill_arrays = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_arrays_delegate>("avutil", "av_image_fill_arrays", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_fill_arrays(ref @dst_data, ref @dst_linesize, @src, @pix_fmt, @width, @height, @align);
        };
        
        vectors.av_image_fill_black = (ref byte_ptrArray4 @dst_data, in long_array4 @dst_linesize, AVPixelFormat @pix_fmt, AVColorRange @range, int @width, int @height) =>
        {
            vectors.av_image_fill_black = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_black_delegate>("avutil", "av_image_fill_black", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_fill_black(ref @dst_data, @dst_linesize, @pix_fmt, @range, @width, @height);
        };
        
        vectors.av_image_fill_linesizes = (ref int_array4 @linesizes, AVPixelFormat @pix_fmt, int @width) =>
        {
            vectors.av_image_fill_linesizes = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_linesizes_delegate>("avutil", "av_image_fill_linesizes", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_fill_linesizes(ref @linesizes, @pix_fmt, @width);
        };
        
        vectors.av_image_fill_max_pixsteps = (ref int_array4 @max_pixsteps, ref int_array4 @max_pixstep_comps, AVPixFmtDescriptor* @pixdesc) =>
        {
            vectors.av_image_fill_max_pixsteps = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_max_pixsteps_delegate>("avutil", "av_image_fill_max_pixsteps", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_image_fill_max_pixsteps(ref @max_pixsteps, ref @max_pixstep_comps, @pixdesc);
        };
        
        vectors.av_image_fill_plane_sizes = (ref ulong_array4 @size, AVPixelFormat @pix_fmt, int @height, in long_array4 @linesizes) =>
        {
            vectors.av_image_fill_plane_sizes = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_plane_sizes_delegate>("avutil", "av_image_fill_plane_sizes", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_fill_plane_sizes(ref @size, @pix_fmt, @height, @linesizes);
        };
        
        vectors.av_image_fill_pointers = (ref byte_ptrArray4 @data, AVPixelFormat @pix_fmt, int @height, byte* @ptr, in int_array4 @linesizes) =>
        {
            vectors.av_image_fill_pointers = FunctionResolver.GetFunctionDelegate<vectors.av_image_fill_pointers_delegate>("avutil", "av_image_fill_pointers", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_fill_pointers(ref @data, @pix_fmt, @height, @ptr, @linesizes);
        };
        
        vectors.av_image_get_buffer_size = (AVPixelFormat @pix_fmt, int @width, int @height, int @align) =>
        {
            vectors.av_image_get_buffer_size = FunctionResolver.GetFunctionDelegate<vectors.av_image_get_buffer_size_delegate>("avutil", "av_image_get_buffer_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_get_buffer_size(@pix_fmt, @width, @height, @align);
        };
        
        vectors.av_image_get_linesize = (AVPixelFormat @pix_fmt, int @width, int @plane) =>
        {
            vectors.av_image_get_linesize = FunctionResolver.GetFunctionDelegate<vectors.av_image_get_linesize_delegate>("avutil", "av_image_get_linesize", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_image_get_linesize(@pix_fmt, @width, @plane);
        };
        
        vectors.av_index_search_timestamp = (AVStream* @st, long @timestamp, int @flags) =>
        {
            vectors.av_index_search_timestamp = FunctionResolver.GetFunctionDelegate<vectors.av_index_search_timestamp_delegate>("avformat", "av_index_search_timestamp", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_index_search_timestamp(@st, @timestamp, @flags);
        };
        
        vectors.av_init_packet = (AVPacket* @pkt) =>
        {
            vectors.av_init_packet = FunctionResolver.GetFunctionDelegate<vectors.av_init_packet_delegate>("avcodec", "av_init_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_init_packet(@pkt);
        };
        
        vectors.av_input_audio_device_next = (AVInputFormat* @d) =>
        {
            vectors.av_input_audio_device_next = FunctionResolver.GetFunctionDelegate<vectors.av_input_audio_device_next_delegate>("avdevice", "av_input_audio_device_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_input_audio_device_next(@d);
        };
        
        vectors.av_input_video_device_next = (AVInputFormat* @d) =>
        {
            vectors.av_input_video_device_next = FunctionResolver.GetFunctionDelegate<vectors.av_input_video_device_next_delegate>("avdevice", "av_input_video_device_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_input_video_device_next(@d);
        };
        
        vectors.av_int_list_length_for_size = (uint @elsize, void* @list, ulong @term) =>
        {
            vectors.av_int_list_length_for_size = FunctionResolver.GetFunctionDelegate<vectors.av_int_list_length_for_size_delegate>("avutil", "av_int_list_length_for_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_int_list_length_for_size(@elsize, @list, @term);
        };
        
        vectors.av_interleaved_write_frame = (AVFormatContext* @s, AVPacket* @pkt) =>
        {
            vectors.av_interleaved_write_frame = FunctionResolver.GetFunctionDelegate<vectors.av_interleaved_write_frame_delegate>("avformat", "av_interleaved_write_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_interleaved_write_frame(@s, @pkt);
        };
        
        vectors.av_interleaved_write_uncoded_frame = (AVFormatContext* @s, int @stream_index, AVFrame* @frame) =>
        {
            vectors.av_interleaved_write_uncoded_frame = FunctionResolver.GetFunctionDelegate<vectors.av_interleaved_write_uncoded_frame_delegate>("avformat", "av_interleaved_write_uncoded_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_interleaved_write_uncoded_frame(@s, @stream_index, @frame);
        };
        
        vectors.av_log = (void* @avcl, int @level, string @fmt) =>
        {
            vectors.av_log = FunctionResolver.GetFunctionDelegate<vectors.av_log_delegate>("avutil", "av_log", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log(@avcl, @level, @fmt);
        };
        
        vectors.av_log_default_callback = (void* @avcl, int @level, string @fmt, byte* @vl) =>
        {
            vectors.av_log_default_callback = FunctionResolver.GetFunctionDelegate<vectors.av_log_default_callback_delegate>("avutil", "av_log_default_callback", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_default_callback(@avcl, @level, @fmt, @vl);
        };
        
        vectors.av_log_format_line = (void* @ptr, int @level, string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix) =>
        {
            vectors.av_log_format_line = FunctionResolver.GetFunctionDelegate<vectors.av_log_format_line_delegate>("avutil", "av_log_format_line", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_format_line(@ptr, @level, @fmt, @vl, @line, @line_size, @print_prefix);
        };
        
        vectors.av_log_format_line2 = (void* @ptr, int @level, string @fmt, byte* @vl, byte* @line, int @line_size, int* @print_prefix) =>
        {
            vectors.av_log_format_line2 = FunctionResolver.GetFunctionDelegate<vectors.av_log_format_line2_delegate>("avutil", "av_log_format_line2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_log_format_line2(@ptr, @level, @fmt, @vl, @line, @line_size, @print_prefix);
        };
        
        vectors.av_log_get_flags = () =>
        {
            vectors.av_log_get_flags = FunctionResolver.GetFunctionDelegate<vectors.av_log_get_flags_delegate>("avutil", "av_log_get_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_log_get_flags();
        };
        
        vectors.av_log_get_level = () =>
        {
            vectors.av_log_get_level = FunctionResolver.GetFunctionDelegate<vectors.av_log_get_level_delegate>("avutil", "av_log_get_level", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_log_get_level();
        };
        
        vectors.av_log_once = (void* @avcl, int @initial_level, int @subsequent_level, int* @state, string @fmt) =>
        {
            vectors.av_log_once = FunctionResolver.GetFunctionDelegate<vectors.av_log_once_delegate>("avutil", "av_log_once", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_once(@avcl, @initial_level, @subsequent_level, @state, @fmt);
        };
        
        vectors.av_log_set_callback = (av_log_set_callback_callback_func @callback) =>
        {
            vectors.av_log_set_callback = FunctionResolver.GetFunctionDelegate<vectors.av_log_set_callback_delegate>("avutil", "av_log_set_callback", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_set_callback(@callback);
        };
        
        vectors.av_log_set_flags = (int @arg) =>
        {
            vectors.av_log_set_flags = FunctionResolver.GetFunctionDelegate<vectors.av_log_set_flags_delegate>("avutil", "av_log_set_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_set_flags(@arg);
        };
        
        vectors.av_log_set_level = (int @level) =>
        {
            vectors.av_log_set_level = FunctionResolver.GetFunctionDelegate<vectors.av_log_set_level_delegate>("avutil", "av_log_set_level", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_log_set_level(@level);
        };
        
        vectors.av_log2 = (uint @v) =>
        {
            vectors.av_log2 = FunctionResolver.GetFunctionDelegate<vectors.av_log2_delegate>("avutil", "av_log2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_log2(@v);
        };
        
        vectors.av_log2_16bit = (uint @v) =>
        {
            vectors.av_log2_16bit = FunctionResolver.GetFunctionDelegate<vectors.av_log2_16bit_delegate>("avutil", "av_log2_16bit", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_log2_16bit(@v);
        };
        
        vectors.av_malloc = (ulong @size) =>
        {
            vectors.av_malloc = FunctionResolver.GetFunctionDelegate<vectors.av_malloc_delegate>("avutil", "av_malloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_malloc(@size);
        };
        
        vectors.av_malloc_array = (ulong @nmemb, ulong @size) =>
        {
            vectors.av_malloc_array = FunctionResolver.GetFunctionDelegate<vectors.av_malloc_array_delegate>("avutil", "av_malloc_array", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_malloc_array(@nmemb, @size);
        };
        
        vectors.av_mallocz = (ulong @size) =>
        {
            vectors.av_mallocz = FunctionResolver.GetFunctionDelegate<vectors.av_mallocz_delegate>("avutil", "av_mallocz", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_mallocz(@size);
        };
        
        vectors.av_mallocz_array = (ulong @nmemb, ulong @size) =>
        {
            vectors.av_mallocz_array = FunctionResolver.GetFunctionDelegate<vectors.av_mallocz_array_delegate>("avutil", "av_mallocz_array", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_mallocz_array(@nmemb, @size);
        };
        
        vectors.av_mastering_display_metadata_alloc = () =>
        {
            vectors.av_mastering_display_metadata_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_mastering_display_metadata_alloc_delegate>("avutil", "av_mastering_display_metadata_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_mastering_display_metadata_alloc();
        };
        
        vectors.av_mastering_display_metadata_create_side_data = (AVFrame* @frame) =>
        {
            vectors.av_mastering_display_metadata_create_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_mastering_display_metadata_create_side_data_delegate>("avutil", "av_mastering_display_metadata_create_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_mastering_display_metadata_create_side_data(@frame);
        };
        
        vectors.av_match_ext = (string @filename, string @extensions) =>
        {
            vectors.av_match_ext = FunctionResolver.GetFunctionDelegate<vectors.av_match_ext_delegate>("avformat", "av_match_ext", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_match_ext(@filename, @extensions);
        };
        
        vectors.av_max_alloc = (ulong @max) =>
        {
            vectors.av_max_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_max_alloc_delegate>("avutil", "av_max_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_max_alloc(@max);
        };
        
        vectors.av_memcpy_backptr = (byte* @dst, int @back, int @cnt) =>
        {
            vectors.av_memcpy_backptr = FunctionResolver.GetFunctionDelegate<vectors.av_memcpy_backptr_delegate>("avutil", "av_memcpy_backptr", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_memcpy_backptr(@dst, @back, @cnt);
        };
        
        vectors.av_memdup = (void* @p, ulong @size) =>
        {
            vectors.av_memdup = FunctionResolver.GetFunctionDelegate<vectors.av_memdup_delegate>("avutil", "av_memdup", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_memdup(@p, @size);
        };
        
        vectors.av_mul_q = (AVRational @b, AVRational @c) =>
        {
            vectors.av_mul_q = FunctionResolver.GetFunctionDelegate<vectors.av_mul_q_delegate>("avutil", "av_mul_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_mul_q(@b, @c);
        };
        
        vectors.av_muxer_iterate = (void** @opaque) =>
        {
            vectors.av_muxer_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_muxer_iterate_delegate>("avformat", "av_muxer_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_muxer_iterate(@opaque);
        };
        
        vectors.av_nearer_q = (AVRational @q, AVRational @q1, AVRational @q2) =>
        {
            vectors.av_nearer_q = FunctionResolver.GetFunctionDelegate<vectors.av_nearer_q_delegate>("avutil", "av_nearer_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_nearer_q(@q, @q1, @q2);
        };
        
        vectors.av_new_packet = (AVPacket* @pkt, int @size) =>
        {
            vectors.av_new_packet = FunctionResolver.GetFunctionDelegate<vectors.av_new_packet_delegate>("avcodec", "av_new_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_new_packet(@pkt, @size);
        };
        
        vectors.av_new_program = (AVFormatContext* @s, int @id) =>
        {
            vectors.av_new_program = FunctionResolver.GetFunctionDelegate<vectors.av_new_program_delegate>("avformat", "av_new_program", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_new_program(@s, @id);
        };
        
        vectors.av_opt_child_class_iterate = (AVClass* @parent, void** @iter) =>
        {
            vectors.av_opt_child_class_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_opt_child_class_iterate_delegate>("avutil", "av_opt_child_class_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_child_class_iterate(@parent, @iter);
        };
        
        vectors.av_opt_child_next = (void* @obj, void* @prev) =>
        {
            vectors.av_opt_child_next = FunctionResolver.GetFunctionDelegate<vectors.av_opt_child_next_delegate>("avutil", "av_opt_child_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_child_next(@obj, @prev);
        };
        
        vectors.av_opt_copy = (void* @dest, void* @src) =>
        {
            vectors.av_opt_copy = FunctionResolver.GetFunctionDelegate<vectors.av_opt_copy_delegate>("avutil", "av_opt_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_copy(@dest, @src);
        };
        
        vectors.av_opt_eval_double = (void* @obj, AVOption* @o, string @val, double* @double_out) =>
        {
            vectors.av_opt_eval_double = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_double_delegate>("avutil", "av_opt_eval_double", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_double(@obj, @o, @val, @double_out);
        };
        
        vectors.av_opt_eval_flags = (void* @obj, AVOption* @o, string @val, int* @flags_out) =>
        {
            vectors.av_opt_eval_flags = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_flags_delegate>("avutil", "av_opt_eval_flags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_flags(@obj, @o, @val, @flags_out);
        };
        
        vectors.av_opt_eval_float = (void* @obj, AVOption* @o, string @val, float* @float_out) =>
        {
            vectors.av_opt_eval_float = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_float_delegate>("avutil", "av_opt_eval_float", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_float(@obj, @o, @val, @float_out);
        };
        
        vectors.av_opt_eval_int = (void* @obj, AVOption* @o, string @val, int* @int_out) =>
        {
            vectors.av_opt_eval_int = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_int_delegate>("avutil", "av_opt_eval_int", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_int(@obj, @o, @val, @int_out);
        };
        
        vectors.av_opt_eval_int64 = (void* @obj, AVOption* @o, string @val, long* @int64_out) =>
        {
            vectors.av_opt_eval_int64 = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_int64_delegate>("avutil", "av_opt_eval_int64", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_int64(@obj, @o, @val, @int64_out);
        };
        
        vectors.av_opt_eval_q = (void* @obj, AVOption* @o, string @val, AVRational* @q_out) =>
        {
            vectors.av_opt_eval_q = FunctionResolver.GetFunctionDelegate<vectors.av_opt_eval_q_delegate>("avutil", "av_opt_eval_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_eval_q(@obj, @o, @val, @q_out);
        };
        
        vectors.av_opt_find = (void* @obj, string @name, string @unit, int @opt_flags, int @search_flags) =>
        {
            vectors.av_opt_find = FunctionResolver.GetFunctionDelegate<vectors.av_opt_find_delegate>("avutil", "av_opt_find", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_find(@obj, @name, @unit, @opt_flags, @search_flags);
        };
        
        vectors.av_opt_find2 = (void* @obj, string @name, string @unit, int @opt_flags, int @search_flags, void** @target_obj) =>
        {
            vectors.av_opt_find2 = FunctionResolver.GetFunctionDelegate<vectors.av_opt_find2_delegate>("avutil", "av_opt_find2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_find2(@obj, @name, @unit, @opt_flags, @search_flags, @target_obj);
        };
        
        vectors.av_opt_flag_is_set = (void* @obj, string @field_name, string @flag_name) =>
        {
            vectors.av_opt_flag_is_set = FunctionResolver.GetFunctionDelegate<vectors.av_opt_flag_is_set_delegate>("avutil", "av_opt_flag_is_set", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_flag_is_set(@obj, @field_name, @flag_name);
        };
        
        vectors.av_opt_free = (void* @obj) =>
        {
            vectors.av_opt_free = FunctionResolver.GetFunctionDelegate<vectors.av_opt_free_delegate>("avutil", "av_opt_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_opt_free(@obj);
        };
        
        vectors.av_opt_freep_ranges = (AVOptionRanges** @ranges) =>
        {
            vectors.av_opt_freep_ranges = FunctionResolver.GetFunctionDelegate<vectors.av_opt_freep_ranges_delegate>("avutil", "av_opt_freep_ranges", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_opt_freep_ranges(@ranges);
        };
        
        vectors.av_opt_get = (void* @obj, string @name, int @search_flags, byte** @out_val) =>
        {
            vectors.av_opt_get = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_delegate>("avutil", "av_opt_get", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_get_channel_layout = (void* @obj, string @name, int @search_flags, long* @ch_layout) =>
        {
            vectors.av_opt_get_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_channel_layout_delegate>("avutil", "av_opt_get_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_channel_layout(@obj, @name, @search_flags, @ch_layout);
        };
        
        vectors.av_opt_get_chlayout = (void* @obj, string @name, int @search_flags, AVChannelLayout* @layout) =>
        {
            vectors.av_opt_get_chlayout = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_chlayout_delegate>("avutil", "av_opt_get_chlayout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_chlayout(@obj, @name, @search_flags, @layout);
        };
        
        vectors.av_opt_get_dict_val = (void* @obj, string @name, int @search_flags, AVDictionary** @out_val) =>
        {
            vectors.av_opt_get_dict_val = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_dict_val_delegate>("avutil", "av_opt_get_dict_val", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_dict_val(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_get_double = (void* @obj, string @name, int @search_flags, double* @out_val) =>
        {
            vectors.av_opt_get_double = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_double_delegate>("avutil", "av_opt_get_double", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_double(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_get_image_size = (void* @obj, string @name, int @search_flags, int* @w_out, int* @h_out) =>
        {
            vectors.av_opt_get_image_size = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_image_size_delegate>("avutil", "av_opt_get_image_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_image_size(@obj, @name, @search_flags, @w_out, @h_out);
        };
        
        vectors.av_opt_get_int = (void* @obj, string @name, int @search_flags, long* @out_val) =>
        {
            vectors.av_opt_get_int = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_int_delegate>("avutil", "av_opt_get_int", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_int(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_get_key_value = (byte** @ropts, string @key_val_sep, string @pairs_sep, uint @flags, byte** @rkey, byte** @rval) =>
        {
            vectors.av_opt_get_key_value = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_key_value_delegate>("avutil", "av_opt_get_key_value", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_key_value(@ropts, @key_val_sep, @pairs_sep, @flags, @rkey, @rval);
        };
        
        vectors.av_opt_get_pixel_fmt = (void* @obj, string @name, int @search_flags, AVPixelFormat* @out_fmt) =>
        {
            vectors.av_opt_get_pixel_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_pixel_fmt_delegate>("avutil", "av_opt_get_pixel_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_pixel_fmt(@obj, @name, @search_flags, @out_fmt);
        };
        
        vectors.av_opt_get_q = (void* @obj, string @name, int @search_flags, AVRational* @out_val) =>
        {
            vectors.av_opt_get_q = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_q_delegate>("avutil", "av_opt_get_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_q(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_get_sample_fmt = (void* @obj, string @name, int @search_flags, AVSampleFormat* @out_fmt) =>
        {
            vectors.av_opt_get_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_sample_fmt_delegate>("avutil", "av_opt_get_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_sample_fmt(@obj, @name, @search_flags, @out_fmt);
        };
        
        vectors.av_opt_get_video_rate = (void* @obj, string @name, int @search_flags, AVRational* @out_val) =>
        {
            vectors.av_opt_get_video_rate = FunctionResolver.GetFunctionDelegate<vectors.av_opt_get_video_rate_delegate>("avutil", "av_opt_get_video_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_get_video_rate(@obj, @name, @search_flags, @out_val);
        };
        
        vectors.av_opt_is_set_to_default = (void* @obj, AVOption* @o) =>
        {
            vectors.av_opt_is_set_to_default = FunctionResolver.GetFunctionDelegate<vectors.av_opt_is_set_to_default_delegate>("avutil", "av_opt_is_set_to_default", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_is_set_to_default(@obj, @o);
        };
        
        vectors.av_opt_is_set_to_default_by_name = (void* @obj, string @name, int @search_flags) =>
        {
            vectors.av_opt_is_set_to_default_by_name = FunctionResolver.GetFunctionDelegate<vectors.av_opt_is_set_to_default_by_name_delegate>("avutil", "av_opt_is_set_to_default_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_is_set_to_default_by_name(@obj, @name, @search_flags);
        };
        
        vectors.av_opt_next = (void* @obj, AVOption* @prev) =>
        {
            vectors.av_opt_next = FunctionResolver.GetFunctionDelegate<vectors.av_opt_next_delegate>("avutil", "av_opt_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_next(@obj, @prev);
        };
        
        vectors.av_opt_ptr = (AVClass* @avclass, void* @obj, string @name) =>
        {
            vectors.av_opt_ptr = FunctionResolver.GetFunctionDelegate<vectors.av_opt_ptr_delegate>("avutil", "av_opt_ptr", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_ptr(@avclass, @obj, @name);
        };
        
        vectors.av_opt_query_ranges = (AVOptionRanges** @p0, void* @obj, string @key, int @flags) =>
        {
            vectors.av_opt_query_ranges = FunctionResolver.GetFunctionDelegate<vectors.av_opt_query_ranges_delegate>("avutil", "av_opt_query_ranges", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_query_ranges(@p0, @obj, @key, @flags);
        };
        
        vectors.av_opt_query_ranges_default = (AVOptionRanges** @p0, void* @obj, string @key, int @flags) =>
        {
            vectors.av_opt_query_ranges_default = FunctionResolver.GetFunctionDelegate<vectors.av_opt_query_ranges_default_delegate>("avutil", "av_opt_query_ranges_default", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_query_ranges_default(@p0, @obj, @key, @flags);
        };
        
        vectors.av_opt_serialize = (void* @obj, int @opt_flags, int @flags, byte** @buffer, byte @key_val_sep, byte @pairs_sep) =>
        {
            vectors.av_opt_serialize = FunctionResolver.GetFunctionDelegate<vectors.av_opt_serialize_delegate>("avutil", "av_opt_serialize", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_serialize(@obj, @opt_flags, @flags, @buffer, @key_val_sep, @pairs_sep);
        };
        
        vectors.av_opt_set = (void* @obj, string @name, string @val, int @search_flags) =>
        {
            vectors.av_opt_set = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_delegate>("avutil", "av_opt_set", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_set_bin = (void* @obj, string @name, byte* @val, int @size, int @search_flags) =>
        {
            vectors.av_opt_set_bin = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_bin_delegate>("avutil", "av_opt_set_bin", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_bin(@obj, @name, @val, @size, @search_flags);
        };
        
        vectors.av_opt_set_channel_layout = (void* @obj, string @name, long @ch_layout, int @search_flags) =>
        {
            vectors.av_opt_set_channel_layout = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_channel_layout_delegate>("avutil", "av_opt_set_channel_layout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_channel_layout(@obj, @name, @ch_layout, @search_flags);
        };
        
        vectors.av_opt_set_chlayout = (void* @obj, string @name, AVChannelLayout* @layout, int @search_flags) =>
        {
            vectors.av_opt_set_chlayout = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_chlayout_delegate>("avutil", "av_opt_set_chlayout", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_chlayout(@obj, @name, @layout, @search_flags);
        };
        
        vectors.av_opt_set_defaults = (void* @s) =>
        {
            vectors.av_opt_set_defaults = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_defaults_delegate>("avutil", "av_opt_set_defaults", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_opt_set_defaults(@s);
        };
        
        vectors.av_opt_set_defaults2 = (void* @s, int @mask, int @flags) =>
        {
            vectors.av_opt_set_defaults2 = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_defaults2_delegate>("avutil", "av_opt_set_defaults2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_opt_set_defaults2(@s, @mask, @flags);
        };
        
        vectors.av_opt_set_dict = (void* @obj, AVDictionary** @options) =>
        {
            vectors.av_opt_set_dict = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_dict_delegate>("avutil", "av_opt_set_dict", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_dict(@obj, @options);
        };
        
        vectors.av_opt_set_dict_val = (void* @obj, string @name, AVDictionary* @val, int @search_flags) =>
        {
            vectors.av_opt_set_dict_val = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_dict_val_delegate>("avutil", "av_opt_set_dict_val", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_dict_val(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_set_dict2 = (void* @obj, AVDictionary** @options, int @search_flags) =>
        {
            vectors.av_opt_set_dict2 = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_dict2_delegate>("avutil", "av_opt_set_dict2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_dict2(@obj, @options, @search_flags);
        };
        
        vectors.av_opt_set_double = (void* @obj, string @name, double @val, int @search_flags) =>
        {
            vectors.av_opt_set_double = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_double_delegate>("avutil", "av_opt_set_double", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_double(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_set_from_string = (void* @ctx, string @opts, byte** @shorthand, string @key_val_sep, string @pairs_sep) =>
        {
            vectors.av_opt_set_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_from_string_delegate>("avutil", "av_opt_set_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_from_string(@ctx, @opts, @shorthand, @key_val_sep, @pairs_sep);
        };
        
        vectors.av_opt_set_image_size = (void* @obj, string @name, int @w, int @h, int @search_flags) =>
        {
            vectors.av_opt_set_image_size = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_image_size_delegate>("avutil", "av_opt_set_image_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_image_size(@obj, @name, @w, @h, @search_flags);
        };
        
        vectors.av_opt_set_int = (void* @obj, string @name, long @val, int @search_flags) =>
        {
            vectors.av_opt_set_int = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_int_delegate>("avutil", "av_opt_set_int", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_int(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_set_pixel_fmt = (void* @obj, string @name, AVPixelFormat @fmt, int @search_flags) =>
        {
            vectors.av_opt_set_pixel_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_pixel_fmt_delegate>("avutil", "av_opt_set_pixel_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_pixel_fmt(@obj, @name, @fmt, @search_flags);
        };
        
        vectors.av_opt_set_q = (void* @obj, string @name, AVRational @val, int @search_flags) =>
        {
            vectors.av_opt_set_q = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_q_delegate>("avutil", "av_opt_set_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_q(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_set_sample_fmt = (void* @obj, string @name, AVSampleFormat @fmt, int @search_flags) =>
        {
            vectors.av_opt_set_sample_fmt = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_sample_fmt_delegate>("avutil", "av_opt_set_sample_fmt", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_sample_fmt(@obj, @name, @fmt, @search_flags);
        };
        
        vectors.av_opt_set_video_rate = (void* @obj, string @name, AVRational @val, int @search_flags) =>
        {
            vectors.av_opt_set_video_rate = FunctionResolver.GetFunctionDelegate<vectors.av_opt_set_video_rate_delegate>("avutil", "av_opt_set_video_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_set_video_rate(@obj, @name, @val, @search_flags);
        };
        
        vectors.av_opt_show2 = (void* @obj, void* @av_log_obj, int @req_flags, int @rej_flags) =>
        {
            vectors.av_opt_show2 = FunctionResolver.GetFunctionDelegate<vectors.av_opt_show2_delegate>("avutil", "av_opt_show2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_opt_show2(@obj, @av_log_obj, @req_flags, @rej_flags);
        };
        
        vectors.av_output_audio_device_next = (AVOutputFormat* @d) =>
        {
            vectors.av_output_audio_device_next = FunctionResolver.GetFunctionDelegate<vectors.av_output_audio_device_next_delegate>("avdevice", "av_output_audio_device_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_output_audio_device_next(@d);
        };
        
        vectors.av_output_video_device_next = (AVOutputFormat* @d) =>
        {
            vectors.av_output_video_device_next = FunctionResolver.GetFunctionDelegate<vectors.av_output_video_device_next_delegate>("avdevice", "av_output_video_device_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_output_video_device_next(@d);
        };
        
        vectors.av_packet_add_side_data = (AVPacket* @pkt, AVPacketSideDataType @type, byte* @data, ulong @size) =>
        {
            vectors.av_packet_add_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_add_side_data_delegate>("avcodec", "av_packet_add_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_add_side_data(@pkt, @type, @data, @size);
        };
        
        vectors.av_packet_alloc = () =>
        {
            vectors.av_packet_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_packet_alloc_delegate>("avcodec", "av_packet_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_alloc();
        };
        
        vectors.av_packet_clone = (AVPacket* @src) =>
        {
            vectors.av_packet_clone = FunctionResolver.GetFunctionDelegate<vectors.av_packet_clone_delegate>("avcodec", "av_packet_clone", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_clone(@src);
        };
        
        vectors.av_packet_copy_props = (AVPacket* @dst, AVPacket* @src) =>
        {
            vectors.av_packet_copy_props = FunctionResolver.GetFunctionDelegate<vectors.av_packet_copy_props_delegate>("avcodec", "av_packet_copy_props", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_copy_props(@dst, @src);
        };
        
        vectors.av_packet_free = (AVPacket** @pkt) =>
        {
            vectors.av_packet_free = FunctionResolver.GetFunctionDelegate<vectors.av_packet_free_delegate>("avcodec", "av_packet_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_packet_free(@pkt);
        };
        
        vectors.av_packet_free_side_data = (AVPacket* @pkt) =>
        {
            vectors.av_packet_free_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_free_side_data_delegate>("avcodec", "av_packet_free_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_packet_free_side_data(@pkt);
        };
        
        vectors.av_packet_from_data = (AVPacket* @pkt, byte* @data, int @size) =>
        {
            vectors.av_packet_from_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_from_data_delegate>("avcodec", "av_packet_from_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_from_data(@pkt, @data, @size);
        };
        
        vectors.av_packet_get_side_data = (AVPacket* @pkt, AVPacketSideDataType @type, ulong* @size) =>
        {
            vectors.av_packet_get_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_get_side_data_delegate>("avcodec", "av_packet_get_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_get_side_data(@pkt, @type, @size);
        };
        
        vectors.av_packet_make_refcounted = (AVPacket* @pkt) =>
        {
            vectors.av_packet_make_refcounted = FunctionResolver.GetFunctionDelegate<vectors.av_packet_make_refcounted_delegate>("avcodec", "av_packet_make_refcounted", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_make_refcounted(@pkt);
        };
        
        vectors.av_packet_make_writable = (AVPacket* @pkt) =>
        {
            vectors.av_packet_make_writable = FunctionResolver.GetFunctionDelegate<vectors.av_packet_make_writable_delegate>("avcodec", "av_packet_make_writable", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_make_writable(@pkt);
        };
        
        vectors.av_packet_move_ref = (AVPacket* @dst, AVPacket* @src) =>
        {
            vectors.av_packet_move_ref = FunctionResolver.GetFunctionDelegate<vectors.av_packet_move_ref_delegate>("avcodec", "av_packet_move_ref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_packet_move_ref(@dst, @src);
        };
        
        vectors.av_packet_new_side_data = (AVPacket* @pkt, AVPacketSideDataType @type, ulong @size) =>
        {
            vectors.av_packet_new_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_new_side_data_delegate>("avcodec", "av_packet_new_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_new_side_data(@pkt, @type, @size);
        };
        
        vectors.av_packet_pack_dictionary = (AVDictionary* @dict, ulong* @size) =>
        {
            vectors.av_packet_pack_dictionary = FunctionResolver.GetFunctionDelegate<vectors.av_packet_pack_dictionary_delegate>("avcodec", "av_packet_pack_dictionary", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_pack_dictionary(@dict, @size);
        };
        
        vectors.av_packet_ref = (AVPacket* @dst, AVPacket* @src) =>
        {
            vectors.av_packet_ref = FunctionResolver.GetFunctionDelegate<vectors.av_packet_ref_delegate>("avcodec", "av_packet_ref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_ref(@dst, @src);
        };
        
        vectors.av_packet_rescale_ts = (AVPacket* @pkt, AVRational @tb_src, AVRational @tb_dst) =>
        {
            vectors.av_packet_rescale_ts = FunctionResolver.GetFunctionDelegate<vectors.av_packet_rescale_ts_delegate>("avcodec", "av_packet_rescale_ts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_packet_rescale_ts(@pkt, @tb_src, @tb_dst);
        };
        
        vectors.av_packet_shrink_side_data = (AVPacket* @pkt, AVPacketSideDataType @type, ulong @size) =>
        {
            vectors.av_packet_shrink_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_packet_shrink_side_data_delegate>("avcodec", "av_packet_shrink_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_shrink_side_data(@pkt, @type, @size);
        };
        
        vectors.av_packet_side_data_name = (AVPacketSideDataType @type) =>
        {
            vectors.av_packet_side_data_name = FunctionResolver.GetFunctionDelegate<vectors.av_packet_side_data_name_delegate>("avcodec", "av_packet_side_data_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_side_data_name(@type);
        };
        
        vectors.av_packet_unpack_dictionary = (byte* @data, ulong @size, AVDictionary** @dict) =>
        {
            vectors.av_packet_unpack_dictionary = FunctionResolver.GetFunctionDelegate<vectors.av_packet_unpack_dictionary_delegate>("avcodec", "av_packet_unpack_dictionary", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_packet_unpack_dictionary(@data, @size, @dict);
        };
        
        vectors.av_packet_unref = (AVPacket* @pkt) =>
        {
            vectors.av_packet_unref = FunctionResolver.GetFunctionDelegate<vectors.av_packet_unref_delegate>("avcodec", "av_packet_unref", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_packet_unref(@pkt);
        };
        
        vectors.av_parse_cpu_caps = (uint* @flags, string @s) =>
        {
            vectors.av_parse_cpu_caps = FunctionResolver.GetFunctionDelegate<vectors.av_parse_cpu_caps_delegate>("avutil", "av_parse_cpu_caps", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_parse_cpu_caps(@flags, @s);
        };
        
        vectors.av_parser_close = (AVCodecParserContext* @s) =>
        {
            vectors.av_parser_close = FunctionResolver.GetFunctionDelegate<vectors.av_parser_close_delegate>("avcodec", "av_parser_close", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_parser_close(@s);
        };
        
        vectors.av_parser_init = (int @codec_id) =>
        {
            vectors.av_parser_init = FunctionResolver.GetFunctionDelegate<vectors.av_parser_init_delegate>("avcodec", "av_parser_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_parser_init(@codec_id);
        };
        
        vectors.av_parser_iterate = (void** @opaque) =>
        {
            vectors.av_parser_iterate = FunctionResolver.GetFunctionDelegate<vectors.av_parser_iterate_delegate>("avcodec", "av_parser_iterate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_parser_iterate(@opaque);
        };
        
        vectors.av_parser_parse2 = (AVCodecParserContext* @s, AVCodecContext* @avctx, byte** @poutbuf, int* @poutbuf_size, byte* @buf, int @buf_size, long @pts, long @dts, long @pos) =>
        {
            vectors.av_parser_parse2 = FunctionResolver.GetFunctionDelegate<vectors.av_parser_parse2_delegate>("avcodec", "av_parser_parse2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_parser_parse2(@s, @avctx, @poutbuf, @poutbuf_size, @buf, @buf_size, @pts, @dts, @pos);
        };
        
        vectors.av_pix_fmt_count_planes = (AVPixelFormat @pix_fmt) =>
        {
            vectors.av_pix_fmt_count_planes = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_count_planes_delegate>("avutil", "av_pix_fmt_count_planes", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_count_planes(@pix_fmt);
        };
        
        vectors.av_pix_fmt_desc_get = (AVPixelFormat @pix_fmt) =>
        {
            vectors.av_pix_fmt_desc_get = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_desc_get_delegate>("avutil", "av_pix_fmt_desc_get", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_desc_get(@pix_fmt);
        };
        
        vectors.av_pix_fmt_desc_get_id = (AVPixFmtDescriptor* @desc) =>
        {
            vectors.av_pix_fmt_desc_get_id = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_desc_get_id_delegate>("avutil", "av_pix_fmt_desc_get_id", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_desc_get_id(@desc);
        };
        
        vectors.av_pix_fmt_desc_next = (AVPixFmtDescriptor* @prev) =>
        {
            vectors.av_pix_fmt_desc_next = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_desc_next_delegate>("avutil", "av_pix_fmt_desc_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_desc_next(@prev);
        };
        
        vectors.av_pix_fmt_get_chroma_sub_sample = (AVPixelFormat @pix_fmt, int* @h_shift, int* @v_shift) =>
        {
            vectors.av_pix_fmt_get_chroma_sub_sample = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_get_chroma_sub_sample_delegate>("avutil", "av_pix_fmt_get_chroma_sub_sample", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_get_chroma_sub_sample(@pix_fmt, @h_shift, @v_shift);
        };
        
        vectors.av_pix_fmt_swap_endianness = (AVPixelFormat @pix_fmt) =>
        {
            vectors.av_pix_fmt_swap_endianness = FunctionResolver.GetFunctionDelegate<vectors.av_pix_fmt_swap_endianness_delegate>("avutil", "av_pix_fmt_swap_endianness", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_pix_fmt_swap_endianness(@pix_fmt);
        };
        
        vectors.av_pkt_dump_log2 = (void* @avcl, int @level, AVPacket* @pkt, int @dump_payload, AVStream* @st) =>
        {
            vectors.av_pkt_dump_log2 = FunctionResolver.GetFunctionDelegate<vectors.av_pkt_dump_log2_delegate>("avformat", "av_pkt_dump_log2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_pkt_dump_log2(@avcl, @level, @pkt, @dump_payload, @st);
        };
        
        vectors.av_pkt_dump2 = (_iobuf* @f, AVPacket* @pkt, int @dump_payload, AVStream* @st) =>
        {
            vectors.av_pkt_dump2 = FunctionResolver.GetFunctionDelegate<vectors.av_pkt_dump2_delegate>("avformat", "av_pkt_dump2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_pkt_dump2(@f, @pkt, @dump_payload, @st);
        };
        
        vectors.av_probe_input_buffer = (AVIOContext* @pb, AVInputFormat** @fmt, string @url, void* @logctx, uint @offset, uint @max_probe_size) =>
        {
            vectors.av_probe_input_buffer = FunctionResolver.GetFunctionDelegate<vectors.av_probe_input_buffer_delegate>("avformat", "av_probe_input_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_probe_input_buffer(@pb, @fmt, @url, @logctx, @offset, @max_probe_size);
        };
        
        vectors.av_probe_input_buffer2 = (AVIOContext* @pb, AVInputFormat** @fmt, string @url, void* @logctx, uint @offset, uint @max_probe_size) =>
        {
            vectors.av_probe_input_buffer2 = FunctionResolver.GetFunctionDelegate<vectors.av_probe_input_buffer2_delegate>("avformat", "av_probe_input_buffer2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_probe_input_buffer2(@pb, @fmt, @url, @logctx, @offset, @max_probe_size);
        };
        
        vectors.av_probe_input_format = (AVProbeData* @pd, int @is_opened) =>
        {
            vectors.av_probe_input_format = FunctionResolver.GetFunctionDelegate<vectors.av_probe_input_format_delegate>("avformat", "av_probe_input_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_probe_input_format(@pd, @is_opened);
        };
        
        vectors.av_probe_input_format2 = (AVProbeData* @pd, int @is_opened, int* @score_max) =>
        {
            vectors.av_probe_input_format2 = FunctionResolver.GetFunctionDelegate<vectors.av_probe_input_format2_delegate>("avformat", "av_probe_input_format2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_probe_input_format2(@pd, @is_opened, @score_max);
        };
        
        vectors.av_probe_input_format3 = (AVProbeData* @pd, int @is_opened, int* @score_ret) =>
        {
            vectors.av_probe_input_format3 = FunctionResolver.GetFunctionDelegate<vectors.av_probe_input_format3_delegate>("avformat", "av_probe_input_format3", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_probe_input_format3(@pd, @is_opened, @score_ret);
        };
        
        vectors.av_program_add_stream_index = (AVFormatContext* @ac, int @progid, uint @idx) =>
        {
            vectors.av_program_add_stream_index = FunctionResolver.GetFunctionDelegate<vectors.av_program_add_stream_index_delegate>("avformat", "av_program_add_stream_index", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_program_add_stream_index(@ac, @progid, @idx);
        };
        
        vectors.av_q2intfloat = (AVRational @q) =>
        {
            vectors.av_q2intfloat = FunctionResolver.GetFunctionDelegate<vectors.av_q2intfloat_delegate>("avutil", "av_q2intfloat", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_q2intfloat(@q);
        };
        
        vectors.av_read_frame = (AVFormatContext* @s, AVPacket* @pkt) =>
        {
            vectors.av_read_frame = FunctionResolver.GetFunctionDelegate<vectors.av_read_frame_delegate>("avformat", "av_read_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_read_frame(@s, @pkt);
        };
        
        vectors.av_read_image_line = (ushort* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component) =>
        {
            vectors.av_read_image_line = FunctionResolver.GetFunctionDelegate<vectors.av_read_image_line_delegate>("avutil", "av_read_image_line", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_read_image_line(@dst, @data, @linesize, @desc, @x, @y, @c, @w, @read_pal_component);
        };
        
        vectors.av_read_image_line2 = (void* @dst, in byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @read_pal_component, int @dst_element_size) =>
        {
            vectors.av_read_image_line2 = FunctionResolver.GetFunctionDelegate<vectors.av_read_image_line2_delegate>("avutil", "av_read_image_line2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_read_image_line2(@dst, @data, @linesize, @desc, @x, @y, @c, @w, @read_pal_component, @dst_element_size);
        };
        
        vectors.av_read_pause = (AVFormatContext* @s) =>
        {
            vectors.av_read_pause = FunctionResolver.GetFunctionDelegate<vectors.av_read_pause_delegate>("avformat", "av_read_pause", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_read_pause(@s);
        };
        
        vectors.av_read_play = (AVFormatContext* @s) =>
        {
            vectors.av_read_play = FunctionResolver.GetFunctionDelegate<vectors.av_read_play_delegate>("avformat", "av_read_play", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_read_play(@s);
        };
        
        vectors.av_realloc = (void* @ptr, ulong @size) =>
        {
            vectors.av_realloc = FunctionResolver.GetFunctionDelegate<vectors.av_realloc_delegate>("avutil", "av_realloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_realloc(@ptr, @size);
        };
        
        vectors.av_realloc_array = (void* @ptr, ulong @nmemb, ulong @size) =>
        {
            vectors.av_realloc_array = FunctionResolver.GetFunctionDelegate<vectors.av_realloc_array_delegate>("avutil", "av_realloc_array", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_realloc_array(@ptr, @nmemb, @size);
        };
        
        vectors.av_realloc_f = (void* @ptr, ulong @nelem, ulong @elsize) =>
        {
            vectors.av_realloc_f = FunctionResolver.GetFunctionDelegate<vectors.av_realloc_f_delegate>("avutil", "av_realloc_f", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_realloc_f(@ptr, @nelem, @elsize);
        };
        
        vectors.av_reallocp = (void* @ptr, ulong @size) =>
        {
            vectors.av_reallocp = FunctionResolver.GetFunctionDelegate<vectors.av_reallocp_delegate>("avutil", "av_reallocp", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_reallocp(@ptr, @size);
        };
        
        vectors.av_reallocp_array = (void* @ptr, ulong @nmemb, ulong @size) =>
        {
            vectors.av_reallocp_array = FunctionResolver.GetFunctionDelegate<vectors.av_reallocp_array_delegate>("avutil", "av_reallocp_array", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_reallocp_array(@ptr, @nmemb, @size);
        };
        
        vectors.av_reduce = (int* @dst_num, int* @dst_den, long @num, long @den, long @max) =>
        {
            vectors.av_reduce = FunctionResolver.GetFunctionDelegate<vectors.av_reduce_delegate>("avutil", "av_reduce", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_reduce(@dst_num, @dst_den, @num, @den, @max);
        };
        
        vectors.av_rescale = (long @a, long @b, long @c) =>
        {
            vectors.av_rescale = FunctionResolver.GetFunctionDelegate<vectors.av_rescale_delegate>("avutil", "av_rescale", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_rescale(@a, @b, @c);
        };
        
        vectors.av_rescale_delta = (AVRational @in_tb, long @in_ts, AVRational @fs_tb, int @duration, long* @last, AVRational @out_tb) =>
        {
            vectors.av_rescale_delta = FunctionResolver.GetFunctionDelegate<vectors.av_rescale_delta_delegate>("avutil", "av_rescale_delta", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_rescale_delta(@in_tb, @in_ts, @fs_tb, @duration, @last, @out_tb);
        };
        
        vectors.av_rescale_q = (long @a, AVRational @bq, AVRational @cq) =>
        {
            vectors.av_rescale_q = FunctionResolver.GetFunctionDelegate<vectors.av_rescale_q_delegate>("avutil", "av_rescale_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_rescale_q(@a, @bq, @cq);
        };
        
        vectors.av_rescale_q_rnd = (long @a, AVRational @bq, AVRational @cq, AVRounding @rnd) =>
        {
            vectors.av_rescale_q_rnd = FunctionResolver.GetFunctionDelegate<vectors.av_rescale_q_rnd_delegate>("avutil", "av_rescale_q_rnd", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_rescale_q_rnd(@a, @bq, @cq, @rnd);
        };
        
        vectors.av_rescale_rnd = (long @a, long @b, long @c, AVRounding @rnd) =>
        {
            vectors.av_rescale_rnd = FunctionResolver.GetFunctionDelegate<vectors.av_rescale_rnd_delegate>("avutil", "av_rescale_rnd", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_rescale_rnd(@a, @b, @c, @rnd);
        };
        
        vectors.av_sample_fmt_is_planar = (AVSampleFormat @sample_fmt) =>
        {
            vectors.av_sample_fmt_is_planar = FunctionResolver.GetFunctionDelegate<vectors.av_sample_fmt_is_planar_delegate>("avutil", "av_sample_fmt_is_planar", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_sample_fmt_is_planar(@sample_fmt);
        };
        
        vectors.av_samples_alloc = (byte** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) =>
        {
            vectors.av_samples_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_samples_alloc_delegate>("avutil", "av_samples_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_alloc(@audio_data, @linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
        };
        
        vectors.av_samples_alloc_array_and_samples = (byte*** @audio_data, int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) =>
        {
            vectors.av_samples_alloc_array_and_samples = FunctionResolver.GetFunctionDelegate<vectors.av_samples_alloc_array_and_samples_delegate>("avutil", "av_samples_alloc_array_and_samples", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_alloc_array_and_samples(@audio_data, @linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
        };
        
        vectors.av_samples_copy = (byte** @dst, byte** @src, int @dst_offset, int @src_offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt) =>
        {
            vectors.av_samples_copy = FunctionResolver.GetFunctionDelegate<vectors.av_samples_copy_delegate>("avutil", "av_samples_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_copy(@dst, @src, @dst_offset, @src_offset, @nb_samples, @nb_channels, @sample_fmt);
        };
        
        vectors.av_samples_fill_arrays = (byte** @audio_data, int* @linesize, byte* @buf, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) =>
        {
            vectors.av_samples_fill_arrays = FunctionResolver.GetFunctionDelegate<vectors.av_samples_fill_arrays_delegate>("avutil", "av_samples_fill_arrays", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_fill_arrays(@audio_data, @linesize, @buf, @nb_channels, @nb_samples, @sample_fmt, @align);
        };
        
        vectors.av_samples_get_buffer_size = (int* @linesize, int @nb_channels, int @nb_samples, AVSampleFormat @sample_fmt, int @align) =>
        {
            vectors.av_samples_get_buffer_size = FunctionResolver.GetFunctionDelegate<vectors.av_samples_get_buffer_size_delegate>("avutil", "av_samples_get_buffer_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_get_buffer_size(@linesize, @nb_channels, @nb_samples, @sample_fmt, @align);
        };
        
        vectors.av_samples_set_silence = (byte** @audio_data, int @offset, int @nb_samples, int @nb_channels, AVSampleFormat @sample_fmt) =>
        {
            vectors.av_samples_set_silence = FunctionResolver.GetFunctionDelegate<vectors.av_samples_set_silence_delegate>("avutil", "av_samples_set_silence", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_samples_set_silence(@audio_data, @offset, @nb_samples, @nb_channels, @sample_fmt);
        };
        
        vectors.av_sdp_create = (AVFormatContext** @ac, int @n_files, byte* @buf, int @size) =>
        {
            vectors.av_sdp_create = FunctionResolver.GetFunctionDelegate<vectors.av_sdp_create_delegate>("avformat", "av_sdp_create", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_sdp_create(@ac, @n_files, @buf, @size);
        };
        
        vectors.av_seek_frame = (AVFormatContext* @s, int @stream_index, long @timestamp, int @flags) =>
        {
            vectors.av_seek_frame = FunctionResolver.GetFunctionDelegate<vectors.av_seek_frame_delegate>("avformat", "av_seek_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_seek_frame(@s, @stream_index, @timestamp, @flags);
        };
        
        vectors.av_set_options_string = (void* @ctx, string @opts, string @key_val_sep, string @pairs_sep) =>
        {
            vectors.av_set_options_string = FunctionResolver.GetFunctionDelegate<vectors.av_set_options_string_delegate>("avutil", "av_set_options_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_set_options_string(@ctx, @opts, @key_val_sep, @pairs_sep);
        };
        
        vectors.av_shrink_packet = (AVPacket* @pkt, int @size) =>
        {
            vectors.av_shrink_packet = FunctionResolver.GetFunctionDelegate<vectors.av_shrink_packet_delegate>("avcodec", "av_shrink_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_shrink_packet(@pkt, @size);
        };
        
        vectors.av_size_mult = (ulong @a, ulong @b, ulong* @r) =>
        {
            vectors.av_size_mult = FunctionResolver.GetFunctionDelegate<vectors.av_size_mult_delegate>("avutil", "av_size_mult", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_size_mult(@a, @b, @r);
        };
        
        vectors.av_strdup = (string @s) =>
        {
            vectors.av_strdup = FunctionResolver.GetFunctionDelegate<vectors.av_strdup_delegate>("avutil", "av_strdup", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_strdup(@s);
        };
        
        vectors.av_stream_add_side_data = (AVStream* @st, AVPacketSideDataType @type, byte* @data, ulong @size) =>
        {
            vectors.av_stream_add_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_stream_add_side_data_delegate>("avformat", "av_stream_add_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_add_side_data(@st, @type, @data, @size);
        };
        
        vectors.av_stream_get_class = () =>
        {
            vectors.av_stream_get_class = FunctionResolver.GetFunctionDelegate<vectors.av_stream_get_class_delegate>("avformat", "av_stream_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_get_class();
        };
        
        vectors.av_stream_get_codec_timebase = (AVStream* @st) =>
        {
            vectors.av_stream_get_codec_timebase = FunctionResolver.GetFunctionDelegate<vectors.av_stream_get_codec_timebase_delegate>("avformat", "av_stream_get_codec_timebase", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_get_codec_timebase(@st);
        };
        
        vectors.av_stream_get_end_pts = (AVStream* @st) =>
        {
            vectors.av_stream_get_end_pts = FunctionResolver.GetFunctionDelegate<vectors.av_stream_get_end_pts_delegate>("avformat", "av_stream_get_end_pts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_get_end_pts(@st);
        };
        
        vectors.av_stream_get_parser = (AVStream* @s) =>
        {
            vectors.av_stream_get_parser = FunctionResolver.GetFunctionDelegate<vectors.av_stream_get_parser_delegate>("avformat", "av_stream_get_parser", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_get_parser(@s);
        };
        
        vectors.av_stream_get_side_data = (AVStream* @stream, AVPacketSideDataType @type, ulong* @size) =>
        {
            vectors.av_stream_get_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_stream_get_side_data_delegate>("avformat", "av_stream_get_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_get_side_data(@stream, @type, @size);
        };
        
        vectors.av_stream_new_side_data = (AVStream* @stream, AVPacketSideDataType @type, ulong @size) =>
        {
            vectors.av_stream_new_side_data = FunctionResolver.GetFunctionDelegate<vectors.av_stream_new_side_data_delegate>("avformat", "av_stream_new_side_data", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_stream_new_side_data(@stream, @type, @size);
        };
        
        vectors.av_strerror = (int @errnum, byte* @errbuf, ulong @errbuf_size) =>
        {
            vectors.av_strerror = FunctionResolver.GetFunctionDelegate<vectors.av_strerror_delegate>("avutil", "av_strerror", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_strerror(@errnum, @errbuf, @errbuf_size);
        };
        
        vectors.av_strndup = (string @s, ulong @len) =>
        {
            vectors.av_strndup = FunctionResolver.GetFunctionDelegate<vectors.av_strndup_delegate>("avutil", "av_strndup", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_strndup(@s, @len);
        };
        
        vectors.av_sub_q = (AVRational @b, AVRational @c) =>
        {
            vectors.av_sub_q = FunctionResolver.GetFunctionDelegate<vectors.av_sub_q_delegate>("avutil", "av_sub_q", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_sub_q(@b, @c);
        };
        
        vectors.av_tempfile = (string @prefix, byte** @filename, int @log_offset, void* @log_ctx) =>
        {
            vectors.av_tempfile = FunctionResolver.GetFunctionDelegate<vectors.av_tempfile_delegate>("avutil", "av_tempfile", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_tempfile(@prefix, @filename, @log_offset, @log_ctx);
        };
        
        vectors.av_timecode_adjust_ntsc_framenum2 = (int @framenum, int @fps) =>
        {
            vectors.av_timecode_adjust_ntsc_framenum2 = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_adjust_ntsc_framenum2_delegate>("avutil", "av_timecode_adjust_ntsc_framenum2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_adjust_ntsc_framenum2(@framenum, @fps);
        };
        
        vectors.av_timecode_check_frame_rate = (AVRational @rate) =>
        {
            vectors.av_timecode_check_frame_rate = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_check_frame_rate_delegate>("avutil", "av_timecode_check_frame_rate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_check_frame_rate(@rate);
        };
        
        vectors.av_timecode_get_smpte = (AVRational @rate, int @drop, int @hh, int @mm, int @ss, int @ff) =>
        {
            vectors.av_timecode_get_smpte = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_get_smpte_delegate>("avutil", "av_timecode_get_smpte", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_get_smpte(@rate, @drop, @hh, @mm, @ss, @ff);
        };
        
        vectors.av_timecode_get_smpte_from_framenum = (AVTimecode* @tc, int @framenum) =>
        {
            vectors.av_timecode_get_smpte_from_framenum = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_get_smpte_from_framenum_delegate>("avutil", "av_timecode_get_smpte_from_framenum", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_get_smpte_from_framenum(@tc, @framenum);
        };
        
        vectors.av_timecode_init = (AVTimecode* @tc, AVRational @rate, int @flags, int @frame_start, void* @log_ctx) =>
        {
            vectors.av_timecode_init = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_init_delegate>("avutil", "av_timecode_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_init(@tc, @rate, @flags, @frame_start, @log_ctx);
        };
        
        vectors.av_timecode_init_from_components = (AVTimecode* @tc, AVRational @rate, int @flags, int @hh, int @mm, int @ss, int @ff, void* @log_ctx) =>
        {
            vectors.av_timecode_init_from_components = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_init_from_components_delegate>("avutil", "av_timecode_init_from_components", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_init_from_components(@tc, @rate, @flags, @hh, @mm, @ss, @ff, @log_ctx);
        };
        
        vectors.av_timecode_init_from_string = (AVTimecode* @tc, AVRational @rate, string @str, void* @log_ctx) =>
        {
            vectors.av_timecode_init_from_string = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_init_from_string_delegate>("avutil", "av_timecode_init_from_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_init_from_string(@tc, @rate, @str, @log_ctx);
        };
        
        vectors.av_timecode_make_mpeg_tc_string = (byte* @buf, uint @tc25bit) =>
        {
            vectors.av_timecode_make_mpeg_tc_string = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_make_mpeg_tc_string_delegate>("avutil", "av_timecode_make_mpeg_tc_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_make_mpeg_tc_string(@buf, @tc25bit);
        };
        
        vectors.av_timecode_make_smpte_tc_string = (byte* @buf, uint @tcsmpte, int @prevent_df) =>
        {
            vectors.av_timecode_make_smpte_tc_string = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_make_smpte_tc_string_delegate>("avutil", "av_timecode_make_smpte_tc_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_make_smpte_tc_string(@buf, @tcsmpte, @prevent_df);
        };
        
        vectors.av_timecode_make_smpte_tc_string2 = (byte* @buf, AVRational @rate, uint @tcsmpte, int @prevent_df, int @skip_field) =>
        {
            vectors.av_timecode_make_smpte_tc_string2 = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_make_smpte_tc_string2_delegate>("avutil", "av_timecode_make_smpte_tc_string2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_make_smpte_tc_string2(@buf, @rate, @tcsmpte, @prevent_df, @skip_field);
        };
        
        vectors.av_timecode_make_string = (AVTimecode* @tc, byte* @buf, int @framenum) =>
        {
            vectors.av_timecode_make_string = FunctionResolver.GetFunctionDelegate<vectors.av_timecode_make_string_delegate>("avutil", "av_timecode_make_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_timecode_make_string(@tc, @buf, @framenum);
        };
        
        vectors.av_tree_destroy = (AVTreeNode* @t) =>
        {
            vectors.av_tree_destroy = FunctionResolver.GetFunctionDelegate<vectors.av_tree_destroy_delegate>("avutil", "av_tree_destroy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_tree_destroy(@t);
        };
        
        vectors.av_tree_enumerate = (AVTreeNode* @t, void* @opaque, av_tree_enumerate_cmp_func @cmp, av_tree_enumerate_enu_func @enu) =>
        {
            vectors.av_tree_enumerate = FunctionResolver.GetFunctionDelegate<vectors.av_tree_enumerate_delegate>("avutil", "av_tree_enumerate", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_tree_enumerate(@t, @opaque, @cmp, @enu);
        };
        
        vectors.av_tree_find = (AVTreeNode* @root, void* @key, av_tree_find_cmp_func @cmp, ref void_ptrArray2 @next) =>
        {
            vectors.av_tree_find = FunctionResolver.GetFunctionDelegate<vectors.av_tree_find_delegate>("avutil", "av_tree_find", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_tree_find(@root, @key, @cmp, ref @next);
        };
        
        vectors.av_tree_insert = (AVTreeNode** @rootp, void* @key, av_tree_insert_cmp_func @cmp, AVTreeNode** @next) =>
        {
            vectors.av_tree_insert = FunctionResolver.GetFunctionDelegate<vectors.av_tree_insert_delegate>("avutil", "av_tree_insert", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_tree_insert(@rootp, @key, @cmp, @next);
        };
        
        vectors.av_tree_node_alloc = () =>
        {
            vectors.av_tree_node_alloc = FunctionResolver.GetFunctionDelegate<vectors.av_tree_node_alloc_delegate>("avutil", "av_tree_node_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_tree_node_alloc();
        };
        
        vectors.av_url_split = (byte* @proto, int @proto_size, byte* @authorization, int @authorization_size, byte* @hostname, int @hostname_size, int* @port_ptr, byte* @path, int @path_size, string @url) =>
        {
            vectors.av_url_split = FunctionResolver.GetFunctionDelegate<vectors.av_url_split_delegate>("avformat", "av_url_split", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_url_split(@proto, @proto_size, @authorization, @authorization_size, @hostname, @hostname_size, @port_ptr, @path, @path_size, @url);
        };
        
        vectors.av_usleep = (uint @usec) =>
        {
            vectors.av_usleep = FunctionResolver.GetFunctionDelegate<vectors.av_usleep_delegate>("avutil", "av_usleep", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_usleep(@usec);
        };
        
        vectors.av_version_info = () =>
        {
            vectors.av_version_info = FunctionResolver.GetFunctionDelegate<vectors.av_version_info_delegate>("avutil", "av_version_info", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_version_info();
        };
        
        vectors.av_vlog = (void* @avcl, int @level, string @fmt, byte* @vl) =>
        {
            vectors.av_vlog = FunctionResolver.GetFunctionDelegate<vectors.av_vlog_delegate>("avutil", "av_vlog", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_vlog(@avcl, @level, @fmt, @vl);
        };
        
        vectors.av_write_frame = (AVFormatContext* @s, AVPacket* @pkt) =>
        {
            vectors.av_write_frame = FunctionResolver.GetFunctionDelegate<vectors.av_write_frame_delegate>("avformat", "av_write_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_write_frame(@s, @pkt);
        };
        
        vectors.av_write_image_line = (ushort* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w) =>
        {
            vectors.av_write_image_line = FunctionResolver.GetFunctionDelegate<vectors.av_write_image_line_delegate>("avutil", "av_write_image_line", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_write_image_line(@src, ref @data, @linesize, @desc, @x, @y, @c, @w);
        };
        
        vectors.av_write_image_line2 = (void* @src, ref byte_ptrArray4 @data, in int_array4 @linesize, AVPixFmtDescriptor* @desc, int @x, int @y, int @c, int @w, int @src_element_size) =>
        {
            vectors.av_write_image_line2 = FunctionResolver.GetFunctionDelegate<vectors.av_write_image_line2_delegate>("avutil", "av_write_image_line2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.av_write_image_line2(@src, ref @data, @linesize, @desc, @x, @y, @c, @w, @src_element_size);
        };
        
        vectors.av_write_trailer = (AVFormatContext* @s) =>
        {
            vectors.av_write_trailer = FunctionResolver.GetFunctionDelegate<vectors.av_write_trailer_delegate>("avformat", "av_write_trailer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_write_trailer(@s);
        };
        
        vectors.av_write_uncoded_frame = (AVFormatContext* @s, int @stream_index, AVFrame* @frame) =>
        {
            vectors.av_write_uncoded_frame = FunctionResolver.GetFunctionDelegate<vectors.av_write_uncoded_frame_delegate>("avformat", "av_write_uncoded_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_write_uncoded_frame(@s, @stream_index, @frame);
        };
        
        vectors.av_write_uncoded_frame_query = (AVFormatContext* @s, int @stream_index) =>
        {
            vectors.av_write_uncoded_frame_query = FunctionResolver.GetFunctionDelegate<vectors.av_write_uncoded_frame_query_delegate>("avformat", "av_write_uncoded_frame_query", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_write_uncoded_frame_query(@s, @stream_index);
        };
        
        vectors.av_xiphlacing = (byte* @s, uint @v) =>
        {
            vectors.av_xiphlacing = FunctionResolver.GetFunctionDelegate<vectors.av_xiphlacing_delegate>("avcodec", "av_xiphlacing", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.av_xiphlacing(@s, @v);
        };
        
        vectors.avcodec_align_dimensions = (AVCodecContext* @s, int* @width, int* @height) =>
        {
            vectors.avcodec_align_dimensions = FunctionResolver.GetFunctionDelegate<vectors.avcodec_align_dimensions_delegate>("avcodec", "avcodec_align_dimensions", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_align_dimensions(@s, @width, @height);
        };
        
        vectors.avcodec_align_dimensions2 = (AVCodecContext* @s, int* @width, int* @height, ref int_array8 @linesize_align) =>
        {
            vectors.avcodec_align_dimensions2 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_align_dimensions2_delegate>("avcodec", "avcodec_align_dimensions2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_align_dimensions2(@s, @width, @height, ref @linesize_align);
        };
        
        vectors.avcodec_alloc_context3 = (AVCodec* @codec) =>
        {
            vectors.avcodec_alloc_context3 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_alloc_context3_delegate>("avcodec", "avcodec_alloc_context3", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_alloc_context3(@codec);
        };
        
        vectors.avcodec_chroma_pos_to_enum = (int @xpos, int @ypos) =>
        {
            vectors.avcodec_chroma_pos_to_enum = FunctionResolver.GetFunctionDelegate<vectors.avcodec_chroma_pos_to_enum_delegate>("avcodec", "avcodec_chroma_pos_to_enum", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_chroma_pos_to_enum(@xpos, @ypos);
        };
        
        vectors.avcodec_close = (AVCodecContext* @avctx) =>
        {
            vectors.avcodec_close = FunctionResolver.GetFunctionDelegate<vectors.avcodec_close_delegate>("avcodec", "avcodec_close", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_close(@avctx);
        };
        
        vectors.avcodec_configuration = () =>
        {
            vectors.avcodec_configuration = FunctionResolver.GetFunctionDelegate<vectors.avcodec_configuration_delegate>("avcodec", "avcodec_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_configuration();
        };
        
        vectors.avcodec_decode_subtitle2 = (AVCodecContext* @avctx, AVSubtitle* @sub, int* @got_sub_ptr, AVPacket* @avpkt) =>
        {
            vectors.avcodec_decode_subtitle2 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_decode_subtitle2_delegate>("avcodec", "avcodec_decode_subtitle2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_decode_subtitle2(@avctx, @sub, @got_sub_ptr, @avpkt);
        };
        
        vectors.avcodec_default_execute = (AVCodecContext* @c, avcodec_default_execute_func_func @func, void* @arg, int* @ret, int @count, int @size) =>
        {
            vectors.avcodec_default_execute = FunctionResolver.GetFunctionDelegate<vectors.avcodec_default_execute_delegate>("avcodec", "avcodec_default_execute", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_default_execute(@c, @func, @arg, @ret, @count, @size);
        };
        
        vectors.avcodec_default_execute2 = (AVCodecContext* @c, avcodec_default_execute2_func_func @func, void* @arg, int* @ret, int @count) =>
        {
            vectors.avcodec_default_execute2 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_default_execute2_delegate>("avcodec", "avcodec_default_execute2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_default_execute2(@c, @func, @arg, @ret, @count);
        };
        
        vectors.avcodec_default_get_buffer2 = (AVCodecContext* @s, AVFrame* @frame, int @flags) =>
        {
            vectors.avcodec_default_get_buffer2 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_default_get_buffer2_delegate>("avcodec", "avcodec_default_get_buffer2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_default_get_buffer2(@s, @frame, @flags);
        };
        
        vectors.avcodec_default_get_encode_buffer = (AVCodecContext* @s, AVPacket* @pkt, int @flags) =>
        {
            vectors.avcodec_default_get_encode_buffer = FunctionResolver.GetFunctionDelegate<vectors.avcodec_default_get_encode_buffer_delegate>("avcodec", "avcodec_default_get_encode_buffer", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_default_get_encode_buffer(@s, @pkt, @flags);
        };
        
        vectors.avcodec_default_get_format = (AVCodecContext* @s, AVPixelFormat* @fmt) =>
        {
            vectors.avcodec_default_get_format = FunctionResolver.GetFunctionDelegate<vectors.avcodec_default_get_format_delegate>("avcodec", "avcodec_default_get_format", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_default_get_format(@s, @fmt);
        };
        
        vectors.avcodec_descriptor_get = (AVCodecID @id) =>
        {
            vectors.avcodec_descriptor_get = FunctionResolver.GetFunctionDelegate<vectors.avcodec_descriptor_get_delegate>("avcodec", "avcodec_descriptor_get", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_descriptor_get(@id);
        };
        
        vectors.avcodec_descriptor_get_by_name = (string @name) =>
        {
            vectors.avcodec_descriptor_get_by_name = FunctionResolver.GetFunctionDelegate<vectors.avcodec_descriptor_get_by_name_delegate>("avcodec", "avcodec_descriptor_get_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_descriptor_get_by_name(@name);
        };
        
        vectors.avcodec_descriptor_next = (AVCodecDescriptor* @prev) =>
        {
            vectors.avcodec_descriptor_next = FunctionResolver.GetFunctionDelegate<vectors.avcodec_descriptor_next_delegate>("avcodec", "avcodec_descriptor_next", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_descriptor_next(@prev);
        };
        
        vectors.avcodec_encode_subtitle = (AVCodecContext* @avctx, byte* @buf, int @buf_size, AVSubtitle* @sub) =>
        {
            vectors.avcodec_encode_subtitle = FunctionResolver.GetFunctionDelegate<vectors.avcodec_encode_subtitle_delegate>("avcodec", "avcodec_encode_subtitle", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_encode_subtitle(@avctx, @buf, @buf_size, @sub);
        };
        
        vectors.avcodec_enum_to_chroma_pos = (int* @xpos, int* @ypos, AVChromaLocation @pos) =>
        {
            vectors.avcodec_enum_to_chroma_pos = FunctionResolver.GetFunctionDelegate<vectors.avcodec_enum_to_chroma_pos_delegate>("avcodec", "avcodec_enum_to_chroma_pos", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_enum_to_chroma_pos(@xpos, @ypos, @pos);
        };
        
        vectors.avcodec_fill_audio_frame = (AVFrame* @frame, int @nb_channels, AVSampleFormat @sample_fmt, byte* @buf, int @buf_size, int @align) =>
        {
            vectors.avcodec_fill_audio_frame = FunctionResolver.GetFunctionDelegate<vectors.avcodec_fill_audio_frame_delegate>("avcodec", "avcodec_fill_audio_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_fill_audio_frame(@frame, @nb_channels, @sample_fmt, @buf, @buf_size, @align);
        };
        
        vectors.avcodec_find_best_pix_fmt_of_list = (AVPixelFormat* @pix_fmt_list, AVPixelFormat @src_pix_fmt, int @has_alpha, int* @loss_ptr) =>
        {
            vectors.avcodec_find_best_pix_fmt_of_list = FunctionResolver.GetFunctionDelegate<vectors.avcodec_find_best_pix_fmt_of_list_delegate>("avcodec", "avcodec_find_best_pix_fmt_of_list", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_find_best_pix_fmt_of_list(@pix_fmt_list, @src_pix_fmt, @has_alpha, @loss_ptr);
        };
        
        vectors.avcodec_find_decoder = (AVCodecID @id) =>
        {
            vectors.avcodec_find_decoder = FunctionResolver.GetFunctionDelegate<vectors.avcodec_find_decoder_delegate>("avcodec", "avcodec_find_decoder", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_find_decoder(@id);
        };
        
        vectors.avcodec_find_decoder_by_name = (string @name) =>
        {
            vectors.avcodec_find_decoder_by_name = FunctionResolver.GetFunctionDelegate<vectors.avcodec_find_decoder_by_name_delegate>("avcodec", "avcodec_find_decoder_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_find_decoder_by_name(@name);
        };
        
        vectors.avcodec_find_encoder = (AVCodecID @id) =>
        {
            vectors.avcodec_find_encoder = FunctionResolver.GetFunctionDelegate<vectors.avcodec_find_encoder_delegate>("avcodec", "avcodec_find_encoder", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_find_encoder(@id);
        };
        
        vectors.avcodec_find_encoder_by_name = (string @name) =>
        {
            vectors.avcodec_find_encoder_by_name = FunctionResolver.GetFunctionDelegate<vectors.avcodec_find_encoder_by_name_delegate>("avcodec", "avcodec_find_encoder_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_find_encoder_by_name(@name);
        };
        
        vectors.avcodec_flush_buffers = (AVCodecContext* @avctx) =>
        {
            vectors.avcodec_flush_buffers = FunctionResolver.GetFunctionDelegate<vectors.avcodec_flush_buffers_delegate>("avcodec", "avcodec_flush_buffers", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_flush_buffers(@avctx);
        };
        
        vectors.avcodec_free_context = (AVCodecContext** @avctx) =>
        {
            vectors.avcodec_free_context = FunctionResolver.GetFunctionDelegate<vectors.avcodec_free_context_delegate>("avcodec", "avcodec_free_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_free_context(@avctx);
        };
        
        vectors.avcodec_get_class = () =>
        {
            vectors.avcodec_get_class = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_class_delegate>("avcodec", "avcodec_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_class();
        };
        
        vectors.avcodec_get_frame_class = () =>
        {
            vectors.avcodec_get_frame_class = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_frame_class_delegate>("avcodec", "avcodec_get_frame_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_frame_class();
        };
        
        vectors.avcodec_get_hw_config = (AVCodec* @codec, int @index) =>
        {
            vectors.avcodec_get_hw_config = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_hw_config_delegate>("avcodec", "avcodec_get_hw_config", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_hw_config(@codec, @index);
        };
        
        vectors.avcodec_get_hw_frames_parameters = (AVCodecContext* @avctx, AVBufferRef* @device_ref, AVPixelFormat @hw_pix_fmt, AVBufferRef** @out_frames_ref) =>
        {
            vectors.avcodec_get_hw_frames_parameters = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_hw_frames_parameters_delegate>("avcodec", "avcodec_get_hw_frames_parameters", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_hw_frames_parameters(@avctx, @device_ref, @hw_pix_fmt, @out_frames_ref);
        };
        
        vectors.avcodec_get_name = (AVCodecID @id) =>
        {
            vectors.avcodec_get_name = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_name_delegate>("avcodec", "avcodec_get_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_name(@id);
        };
        
        vectors.avcodec_get_subtitle_rect_class = () =>
        {
            vectors.avcodec_get_subtitle_rect_class = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_subtitle_rect_class_delegate>("avcodec", "avcodec_get_subtitle_rect_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_subtitle_rect_class();
        };
        
        vectors.avcodec_get_type = (AVCodecID @codec_id) =>
        {
            vectors.avcodec_get_type = FunctionResolver.GetFunctionDelegate<vectors.avcodec_get_type_delegate>("avcodec", "avcodec_get_type", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_get_type(@codec_id);
        };
        
        vectors.avcodec_is_open = (AVCodecContext* @s) =>
        {
            vectors.avcodec_is_open = FunctionResolver.GetFunctionDelegate<vectors.avcodec_is_open_delegate>("avcodec", "avcodec_is_open", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_is_open(@s);
        };
        
        vectors.avcodec_license = () =>
        {
            vectors.avcodec_license = FunctionResolver.GetFunctionDelegate<vectors.avcodec_license_delegate>("avcodec", "avcodec_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_license();
        };
        
        vectors.avcodec_open2 = (AVCodecContext* @avctx, AVCodec* @codec, AVDictionary** @options) =>
        {
            vectors.avcodec_open2 = FunctionResolver.GetFunctionDelegate<vectors.avcodec_open2_delegate>("avcodec", "avcodec_open2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_open2(@avctx, @codec, @options);
        };
        
        vectors.avcodec_parameters_alloc = () =>
        {
            vectors.avcodec_parameters_alloc = FunctionResolver.GetFunctionDelegate<vectors.avcodec_parameters_alloc_delegate>("avcodec", "avcodec_parameters_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_parameters_alloc();
        };
        
        vectors.avcodec_parameters_copy = (AVCodecParameters* @dst, AVCodecParameters* @src) =>
        {
            vectors.avcodec_parameters_copy = FunctionResolver.GetFunctionDelegate<vectors.avcodec_parameters_copy_delegate>("avcodec", "avcodec_parameters_copy", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_parameters_copy(@dst, @src);
        };
        
        vectors.avcodec_parameters_free = (AVCodecParameters** @par) =>
        {
            vectors.avcodec_parameters_free = FunctionResolver.GetFunctionDelegate<vectors.avcodec_parameters_free_delegate>("avcodec", "avcodec_parameters_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_parameters_free(@par);
        };
        
        vectors.avcodec_parameters_from_context = (AVCodecParameters* @par, AVCodecContext* @codec) =>
        {
            vectors.avcodec_parameters_from_context = FunctionResolver.GetFunctionDelegate<vectors.avcodec_parameters_from_context_delegate>("avcodec", "avcodec_parameters_from_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_parameters_from_context(@par, @codec);
        };
        
        vectors.avcodec_parameters_to_context = (AVCodecContext* @codec, AVCodecParameters* @par) =>
        {
            vectors.avcodec_parameters_to_context = FunctionResolver.GetFunctionDelegate<vectors.avcodec_parameters_to_context_delegate>("avcodec", "avcodec_parameters_to_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_parameters_to_context(@codec, @par);
        };
        
        vectors.avcodec_pix_fmt_to_codec_tag = (AVPixelFormat @pix_fmt) =>
        {
            vectors.avcodec_pix_fmt_to_codec_tag = FunctionResolver.GetFunctionDelegate<vectors.avcodec_pix_fmt_to_codec_tag_delegate>("avcodec", "avcodec_pix_fmt_to_codec_tag", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_pix_fmt_to_codec_tag(@pix_fmt);
        };
        
        vectors.avcodec_profile_name = (AVCodecID @codec_id, int @profile) =>
        {
            vectors.avcodec_profile_name = FunctionResolver.GetFunctionDelegate<vectors.avcodec_profile_name_delegate>("avcodec", "avcodec_profile_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_profile_name(@codec_id, @profile);
        };
        
        vectors.avcodec_receive_frame = (AVCodecContext* @avctx, AVFrame* @frame) =>
        {
            vectors.avcodec_receive_frame = FunctionResolver.GetFunctionDelegate<vectors.avcodec_receive_frame_delegate>("avcodec", "avcodec_receive_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_receive_frame(@avctx, @frame);
        };
        
        vectors.avcodec_receive_packet = (AVCodecContext* @avctx, AVPacket* @avpkt) =>
        {
            vectors.avcodec_receive_packet = FunctionResolver.GetFunctionDelegate<vectors.avcodec_receive_packet_delegate>("avcodec", "avcodec_receive_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_receive_packet(@avctx, @avpkt);
        };
        
        vectors.avcodec_send_frame = (AVCodecContext* @avctx, AVFrame* @frame) =>
        {
            vectors.avcodec_send_frame = FunctionResolver.GetFunctionDelegate<vectors.avcodec_send_frame_delegate>("avcodec", "avcodec_send_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_send_frame(@avctx, @frame);
        };
        
        vectors.avcodec_send_packet = (AVCodecContext* @avctx, AVPacket* @avpkt) =>
        {
            vectors.avcodec_send_packet = FunctionResolver.GetFunctionDelegate<vectors.avcodec_send_packet_delegate>("avcodec", "avcodec_send_packet", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_send_packet(@avctx, @avpkt);
        };
        
        vectors.avcodec_string = (byte* @buf, int @buf_size, AVCodecContext* @enc, int @encode) =>
        {
            vectors.avcodec_string = FunctionResolver.GetFunctionDelegate<vectors.avcodec_string_delegate>("avcodec", "avcodec_string", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avcodec_string(@buf, @buf_size, @enc, @encode);
        };
        
        vectors.avcodec_version = () =>
        {
            vectors.avcodec_version = FunctionResolver.GetFunctionDelegate<vectors.avcodec_version_delegate>("avcodec", "avcodec_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avcodec_version();
        };
        
        vectors.avdevice_app_to_dev_control_message = (AVFormatContext* @s, AVAppToDevMessageType @type, void* @data, ulong @data_size) =>
        {
            vectors.avdevice_app_to_dev_control_message = FunctionResolver.GetFunctionDelegate<vectors.avdevice_app_to_dev_control_message_delegate>("avdevice", "avdevice_app_to_dev_control_message", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_app_to_dev_control_message(@s, @type, @data, @data_size);
        };
        
        vectors.avdevice_capabilities_create = (AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s, AVDictionary** @device_options) =>
        {
            vectors.avdevice_capabilities_create = FunctionResolver.GetFunctionDelegate<vectors.avdevice_capabilities_create_delegate>("avdevice", "avdevice_capabilities_create", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_capabilities_create(@caps, @s, @device_options);
        };
        
        vectors.avdevice_capabilities_free = (AVDeviceCapabilitiesQuery** @caps, AVFormatContext* @s) =>
        {
            vectors.avdevice_capabilities_free = FunctionResolver.GetFunctionDelegate<vectors.avdevice_capabilities_free_delegate>("avdevice", "avdevice_capabilities_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avdevice_capabilities_free(@caps, @s);
        };
        
        vectors.avdevice_configuration = () =>
        {
            vectors.avdevice_configuration = FunctionResolver.GetFunctionDelegate<vectors.avdevice_configuration_delegate>("avdevice", "avdevice_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_configuration();
        };
        
        vectors.avdevice_dev_to_app_control_message = (AVFormatContext* @s, AVDevToAppMessageType @type, void* @data, ulong @data_size) =>
        {
            vectors.avdevice_dev_to_app_control_message = FunctionResolver.GetFunctionDelegate<vectors.avdevice_dev_to_app_control_message_delegate>("avdevice", "avdevice_dev_to_app_control_message", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_dev_to_app_control_message(@s, @type, @data, @data_size);
        };
        
        vectors.avdevice_free_list_devices = (AVDeviceInfoList** @device_list) =>
        {
            vectors.avdevice_free_list_devices = FunctionResolver.GetFunctionDelegate<vectors.avdevice_free_list_devices_delegate>("avdevice", "avdevice_free_list_devices", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avdevice_free_list_devices(@device_list);
        };
        
        vectors.avdevice_license = () =>
        {
            vectors.avdevice_license = FunctionResolver.GetFunctionDelegate<vectors.avdevice_license_delegate>("avdevice", "avdevice_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_license();
        };
        
        vectors.avdevice_list_devices = (AVFormatContext* @s, AVDeviceInfoList** @device_list) =>
        {
            vectors.avdevice_list_devices = FunctionResolver.GetFunctionDelegate<vectors.avdevice_list_devices_delegate>("avdevice", "avdevice_list_devices", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_list_devices(@s, @device_list);
        };
        
        vectors.avdevice_list_input_sources = (AVInputFormat* @device, string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list) =>
        {
            vectors.avdevice_list_input_sources = FunctionResolver.GetFunctionDelegate<vectors.avdevice_list_input_sources_delegate>("avdevice", "avdevice_list_input_sources", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_list_input_sources(@device, @device_name, @device_options, @device_list);
        };
        
        vectors.avdevice_list_output_sinks = (AVOutputFormat* @device, string @device_name, AVDictionary* @device_options, AVDeviceInfoList** @device_list) =>
        {
            vectors.avdevice_list_output_sinks = FunctionResolver.GetFunctionDelegate<vectors.avdevice_list_output_sinks_delegate>("avdevice", "avdevice_list_output_sinks", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_list_output_sinks(@device, @device_name, @device_options, @device_list);
        };
        
        vectors.avdevice_register_all = () =>
        {
            vectors.avdevice_register_all = FunctionResolver.GetFunctionDelegate<vectors.avdevice_register_all_delegate>("avdevice", "avdevice_register_all", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avdevice_register_all();
        };
        
        vectors.avdevice_version = () =>
        {
            vectors.avdevice_version = FunctionResolver.GetFunctionDelegate<vectors.avdevice_version_delegate>("avdevice", "avdevice_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avdevice_version();
        };
        
        vectors.avfilter_config_links = (AVFilterContext* @filter) =>
        {
            vectors.avfilter_config_links = FunctionResolver.GetFunctionDelegate<vectors.avfilter_config_links_delegate>("avfilter", "avfilter_config_links", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_config_links(@filter);
        };
        
        vectors.avfilter_configuration = () =>
        {
            vectors.avfilter_configuration = FunctionResolver.GetFunctionDelegate<vectors.avfilter_configuration_delegate>("avfilter", "avfilter_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_configuration();
        };
        
        vectors.avfilter_filter_pad_count = (AVFilter* @filter, int @is_output) =>
        {
            vectors.avfilter_filter_pad_count = FunctionResolver.GetFunctionDelegate<vectors.avfilter_filter_pad_count_delegate>("avfilter", "avfilter_filter_pad_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_filter_pad_count(@filter, @is_output);
        };
        
        vectors.avfilter_free = (AVFilterContext* @filter) =>
        {
            vectors.avfilter_free = FunctionResolver.GetFunctionDelegate<vectors.avfilter_free_delegate>("avfilter", "avfilter_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avfilter_free(@filter);
        };
        
        vectors.avfilter_get_by_name = (string @name) =>
        {
            vectors.avfilter_get_by_name = FunctionResolver.GetFunctionDelegate<vectors.avfilter_get_by_name_delegate>("avfilter", "avfilter_get_by_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_get_by_name(@name);
        };
        
        vectors.avfilter_get_class = () =>
        {
            vectors.avfilter_get_class = FunctionResolver.GetFunctionDelegate<vectors.avfilter_get_class_delegate>("avfilter", "avfilter_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_get_class();
        };
        
        vectors.avfilter_graph_alloc = () =>
        {
            vectors.avfilter_graph_alloc = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_alloc_delegate>("avfilter", "avfilter_graph_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_alloc();
        };
        
        vectors.avfilter_graph_alloc_filter = (AVFilterGraph* @graph, AVFilter* @filter, string @name) =>
        {
            vectors.avfilter_graph_alloc_filter = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_alloc_filter_delegate>("avfilter", "avfilter_graph_alloc_filter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_alloc_filter(@graph, @filter, @name);
        };
        
        vectors.avfilter_graph_config = (AVFilterGraph* @graphctx, void* @log_ctx) =>
        {
            vectors.avfilter_graph_config = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_config_delegate>("avfilter", "avfilter_graph_config", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_config(@graphctx, @log_ctx);
        };
        
        vectors.avfilter_graph_create_filter = (AVFilterContext** @filt_ctx, AVFilter* @filt, string @name, string @args, void* @opaque, AVFilterGraph* @graph_ctx) =>
        {
            vectors.avfilter_graph_create_filter = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_create_filter_delegate>("avfilter", "avfilter_graph_create_filter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_create_filter(@filt_ctx, @filt, @name, @args, @opaque, @graph_ctx);
        };
        
        vectors.avfilter_graph_dump = (AVFilterGraph* @graph, string @options) =>
        {
            vectors.avfilter_graph_dump = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_dump_delegate>("avfilter", "avfilter_graph_dump", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_dump(@graph, @options);
        };
        
        vectors.avfilter_graph_free = (AVFilterGraph** @graph) =>
        {
            vectors.avfilter_graph_free = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_free_delegate>("avfilter", "avfilter_graph_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avfilter_graph_free(@graph);
        };
        
        vectors.avfilter_graph_get_filter = (AVFilterGraph* @graph, string @name) =>
        {
            vectors.avfilter_graph_get_filter = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_get_filter_delegate>("avfilter", "avfilter_graph_get_filter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_get_filter(@graph, @name);
        };
        
        vectors.avfilter_graph_parse = (AVFilterGraph* @graph, string @filters, AVFilterInOut* @inputs, AVFilterInOut* @outputs, void* @log_ctx) =>
        {
            vectors.avfilter_graph_parse = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_parse_delegate>("avfilter", "avfilter_graph_parse", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_parse(@graph, @filters, @inputs, @outputs, @log_ctx);
        };
        
        vectors.avfilter_graph_parse_ptr = (AVFilterGraph* @graph, string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs, void* @log_ctx) =>
        {
            vectors.avfilter_graph_parse_ptr = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_parse_ptr_delegate>("avfilter", "avfilter_graph_parse_ptr", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_parse_ptr(@graph, @filters, @inputs, @outputs, @log_ctx);
        };
        
        vectors.avfilter_graph_parse2 = (AVFilterGraph* @graph, string @filters, AVFilterInOut** @inputs, AVFilterInOut** @outputs) =>
        {
            vectors.avfilter_graph_parse2 = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_parse2_delegate>("avfilter", "avfilter_graph_parse2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_parse2(@graph, @filters, @inputs, @outputs);
        };
        
        vectors.avfilter_graph_queue_command = (AVFilterGraph* @graph, string @target, string @cmd, string @arg, int @flags, double @ts) =>
        {
            vectors.avfilter_graph_queue_command = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_queue_command_delegate>("avfilter", "avfilter_graph_queue_command", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_queue_command(@graph, @target, @cmd, @arg, @flags, @ts);
        };
        
        vectors.avfilter_graph_request_oldest = (AVFilterGraph* @graph) =>
        {
            vectors.avfilter_graph_request_oldest = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_request_oldest_delegate>("avfilter", "avfilter_graph_request_oldest", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_request_oldest(@graph);
        };
        
        vectors.avfilter_graph_send_command = (AVFilterGraph* @graph, string @target, string @cmd, string @arg, byte* @res, int @res_len, int @flags) =>
        {
            vectors.avfilter_graph_send_command = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_send_command_delegate>("avfilter", "avfilter_graph_send_command", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_graph_send_command(@graph, @target, @cmd, @arg, @res, @res_len, @flags);
        };
        
        vectors.avfilter_graph_set_auto_convert = (AVFilterGraph* @graph, uint @flags) =>
        {
            vectors.avfilter_graph_set_auto_convert = FunctionResolver.GetFunctionDelegate<vectors.avfilter_graph_set_auto_convert_delegate>("avfilter", "avfilter_graph_set_auto_convert", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avfilter_graph_set_auto_convert(@graph, @flags);
        };
        
        vectors.avfilter_init_dict = (AVFilterContext* @ctx, AVDictionary** @options) =>
        {
            vectors.avfilter_init_dict = FunctionResolver.GetFunctionDelegate<vectors.avfilter_init_dict_delegate>("avfilter", "avfilter_init_dict", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_init_dict(@ctx, @options);
        };
        
        vectors.avfilter_init_str = (AVFilterContext* @ctx, string @args) =>
        {
            vectors.avfilter_init_str = FunctionResolver.GetFunctionDelegate<vectors.avfilter_init_str_delegate>("avfilter", "avfilter_init_str", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_init_str(@ctx, @args);
        };
        
        vectors.avfilter_inout_alloc = () =>
        {
            vectors.avfilter_inout_alloc = FunctionResolver.GetFunctionDelegate<vectors.avfilter_inout_alloc_delegate>("avfilter", "avfilter_inout_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_inout_alloc();
        };
        
        vectors.avfilter_inout_free = (AVFilterInOut** @inout) =>
        {
            vectors.avfilter_inout_free = FunctionResolver.GetFunctionDelegate<vectors.avfilter_inout_free_delegate>("avfilter", "avfilter_inout_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avfilter_inout_free(@inout);
        };
        
        vectors.avfilter_insert_filter = (AVFilterLink* @link, AVFilterContext* @filt, uint @filt_srcpad_idx, uint @filt_dstpad_idx) =>
        {
            vectors.avfilter_insert_filter = FunctionResolver.GetFunctionDelegate<vectors.avfilter_insert_filter_delegate>("avfilter", "avfilter_insert_filter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_insert_filter(@link, @filt, @filt_srcpad_idx, @filt_dstpad_idx);
        };
        
        vectors.avfilter_license = () =>
        {
            vectors.avfilter_license = FunctionResolver.GetFunctionDelegate<vectors.avfilter_license_delegate>("avfilter", "avfilter_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_license();
        };
        
        vectors.avfilter_link = (AVFilterContext* @src, uint @srcpad, AVFilterContext* @dst, uint @dstpad) =>
        {
            vectors.avfilter_link = FunctionResolver.GetFunctionDelegate<vectors.avfilter_link_delegate>("avfilter", "avfilter_link", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_link(@src, @srcpad, @dst, @dstpad);
        };
        
        vectors.avfilter_link_free = (AVFilterLink** @link) =>
        {
            vectors.avfilter_link_free = FunctionResolver.GetFunctionDelegate<vectors.avfilter_link_free_delegate>("avfilter", "avfilter_link_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avfilter_link_free(@link);
        };
        
        vectors.avfilter_pad_count = (AVFilterPad* @pads) =>
        {
            vectors.avfilter_pad_count = FunctionResolver.GetFunctionDelegate<vectors.avfilter_pad_count_delegate>("avfilter", "avfilter_pad_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_pad_count(@pads);
        };
        
        vectors.avfilter_pad_get_name = (AVFilterPad* @pads, int @pad_idx) =>
        {
            vectors.avfilter_pad_get_name = FunctionResolver.GetFunctionDelegate<vectors.avfilter_pad_get_name_delegate>("avfilter", "avfilter_pad_get_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_pad_get_name(@pads, @pad_idx);
        };
        
        vectors.avfilter_pad_get_type = (AVFilterPad* @pads, int @pad_idx) =>
        {
            vectors.avfilter_pad_get_type = FunctionResolver.GetFunctionDelegate<vectors.avfilter_pad_get_type_delegate>("avfilter", "avfilter_pad_get_type", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_pad_get_type(@pads, @pad_idx);
        };
        
        vectors.avfilter_process_command = (AVFilterContext* @filter, string @cmd, string @arg, byte* @res, int @res_len, int @flags) =>
        {
            vectors.avfilter_process_command = FunctionResolver.GetFunctionDelegate<vectors.avfilter_process_command_delegate>("avfilter", "avfilter_process_command", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_process_command(@filter, @cmd, @arg, @res, @res_len, @flags);
        };
        
        vectors.avfilter_version = () =>
        {
            vectors.avfilter_version = FunctionResolver.GetFunctionDelegate<vectors.avfilter_version_delegate>("avfilter", "avfilter_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avfilter_version();
        };
        
        vectors.avformat_alloc_context = () =>
        {
            vectors.avformat_alloc_context = FunctionResolver.GetFunctionDelegate<vectors.avformat_alloc_context_delegate>("avformat", "avformat_alloc_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_alloc_context();
        };
        
        vectors.avformat_alloc_output_context2 = (AVFormatContext** @ctx, AVOutputFormat* @oformat, string @format_name, string @filename) =>
        {
            vectors.avformat_alloc_output_context2 = FunctionResolver.GetFunctionDelegate<vectors.avformat_alloc_output_context2_delegate>("avformat", "avformat_alloc_output_context2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_alloc_output_context2(@ctx, @oformat, @format_name, @filename);
        };
        
        vectors.avformat_close_input = (AVFormatContext** @s) =>
        {
            vectors.avformat_close_input = FunctionResolver.GetFunctionDelegate<vectors.avformat_close_input_delegate>("avformat", "avformat_close_input", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avformat_close_input(@s);
        };
        
        vectors.avformat_configuration = () =>
        {
            vectors.avformat_configuration = FunctionResolver.GetFunctionDelegate<vectors.avformat_configuration_delegate>("avformat", "avformat_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_configuration();
        };
        
        vectors.avformat_find_stream_info = (AVFormatContext* @ic, AVDictionary** @options) =>
        {
            vectors.avformat_find_stream_info = FunctionResolver.GetFunctionDelegate<vectors.avformat_find_stream_info_delegate>("avformat", "avformat_find_stream_info", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_find_stream_info(@ic, @options);
        };
        
        vectors.avformat_flush = (AVFormatContext* @s) =>
        {
            vectors.avformat_flush = FunctionResolver.GetFunctionDelegate<vectors.avformat_flush_delegate>("avformat", "avformat_flush", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_flush(@s);
        };
        
        vectors.avformat_free_context = (AVFormatContext* @s) =>
        {
            vectors.avformat_free_context = FunctionResolver.GetFunctionDelegate<vectors.avformat_free_context_delegate>("avformat", "avformat_free_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avformat_free_context(@s);
        };
        
        vectors.avformat_get_class = () =>
        {
            vectors.avformat_get_class = FunctionResolver.GetFunctionDelegate<vectors.avformat_get_class_delegate>("avformat", "avformat_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_get_class();
        };
        
        vectors.avformat_get_mov_audio_tags = () =>
        {
            vectors.avformat_get_mov_audio_tags = FunctionResolver.GetFunctionDelegate<vectors.avformat_get_mov_audio_tags_delegate>("avformat", "avformat_get_mov_audio_tags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_get_mov_audio_tags();
        };
        
        vectors.avformat_get_mov_video_tags = () =>
        {
            vectors.avformat_get_mov_video_tags = FunctionResolver.GetFunctionDelegate<vectors.avformat_get_mov_video_tags_delegate>("avformat", "avformat_get_mov_video_tags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_get_mov_video_tags();
        };
        
        vectors.avformat_get_riff_audio_tags = () =>
        {
            vectors.avformat_get_riff_audio_tags = FunctionResolver.GetFunctionDelegate<vectors.avformat_get_riff_audio_tags_delegate>("avformat", "avformat_get_riff_audio_tags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_get_riff_audio_tags();
        };
        
        vectors.avformat_get_riff_video_tags = () =>
        {
            vectors.avformat_get_riff_video_tags = FunctionResolver.GetFunctionDelegate<vectors.avformat_get_riff_video_tags_delegate>("avformat", "avformat_get_riff_video_tags", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_get_riff_video_tags();
        };
        
        vectors.avformat_index_get_entries_count = (AVStream* @st) =>
        {
            vectors.avformat_index_get_entries_count = FunctionResolver.GetFunctionDelegate<vectors.avformat_index_get_entries_count_delegate>("avformat", "avformat_index_get_entries_count", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_index_get_entries_count(@st);
        };
        
        vectors.avformat_index_get_entry = (AVStream* @st, int @idx) =>
        {
            vectors.avformat_index_get_entry = FunctionResolver.GetFunctionDelegate<vectors.avformat_index_get_entry_delegate>("avformat", "avformat_index_get_entry", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_index_get_entry(@st, @idx);
        };
        
        vectors.avformat_index_get_entry_from_timestamp = (AVStream* @st, long @wanted_timestamp, int @flags) =>
        {
            vectors.avformat_index_get_entry_from_timestamp = FunctionResolver.GetFunctionDelegate<vectors.avformat_index_get_entry_from_timestamp_delegate>("avformat", "avformat_index_get_entry_from_timestamp", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_index_get_entry_from_timestamp(@st, @wanted_timestamp, @flags);
        };
        
        vectors.avformat_init_output = (AVFormatContext* @s, AVDictionary** @options) =>
        {
            vectors.avformat_init_output = FunctionResolver.GetFunctionDelegate<vectors.avformat_init_output_delegate>("avformat", "avformat_init_output", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_init_output(@s, @options);
        };
        
        vectors.avformat_license = () =>
        {
            vectors.avformat_license = FunctionResolver.GetFunctionDelegate<vectors.avformat_license_delegate>("avformat", "avformat_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_license();
        };
        
        vectors.avformat_match_stream_specifier = (AVFormatContext* @s, AVStream* @st, string @spec) =>
        {
            vectors.avformat_match_stream_specifier = FunctionResolver.GetFunctionDelegate<vectors.avformat_match_stream_specifier_delegate>("avformat", "avformat_match_stream_specifier", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_match_stream_specifier(@s, @st, @spec);
        };
        
        vectors.avformat_network_deinit = () =>
        {
            vectors.avformat_network_deinit = FunctionResolver.GetFunctionDelegate<vectors.avformat_network_deinit_delegate>("avformat", "avformat_network_deinit", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_network_deinit();
        };
        
        vectors.avformat_network_init = () =>
        {
            vectors.avformat_network_init = FunctionResolver.GetFunctionDelegate<vectors.avformat_network_init_delegate>("avformat", "avformat_network_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_network_init();
        };
        
        vectors.avformat_new_stream = (AVFormatContext* @s, AVCodec* @c) =>
        {
            vectors.avformat_new_stream = FunctionResolver.GetFunctionDelegate<vectors.avformat_new_stream_delegate>("avformat", "avformat_new_stream", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_new_stream(@s, @c);
        };
        
        vectors.avformat_open_input = (AVFormatContext** @ps, string @url, AVInputFormat* @fmt, AVDictionary** @options) =>
        {
            vectors.avformat_open_input = FunctionResolver.GetFunctionDelegate<vectors.avformat_open_input_delegate>("avformat", "avformat_open_input", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_open_input(@ps, @url, @fmt, @options);
        };
        
        vectors.avformat_query_codec = (AVOutputFormat* @ofmt, AVCodecID @codec_id, int @std_compliance) =>
        {
            vectors.avformat_query_codec = FunctionResolver.GetFunctionDelegate<vectors.avformat_query_codec_delegate>("avformat", "avformat_query_codec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_query_codec(@ofmt, @codec_id, @std_compliance);
        };
        
        vectors.avformat_queue_attached_pictures = (AVFormatContext* @s) =>
        {
            vectors.avformat_queue_attached_pictures = FunctionResolver.GetFunctionDelegate<vectors.avformat_queue_attached_pictures_delegate>("avformat", "avformat_queue_attached_pictures", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_queue_attached_pictures(@s);
        };
        
        vectors.avformat_seek_file = (AVFormatContext* @s, int @stream_index, long @min_ts, long @ts, long @max_ts, int @flags) =>
        {
            vectors.avformat_seek_file = FunctionResolver.GetFunctionDelegate<vectors.avformat_seek_file_delegate>("avformat", "avformat_seek_file", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_seek_file(@s, @stream_index, @min_ts, @ts, @max_ts, @flags);
        };
        
        vectors.avformat_transfer_internal_stream_timing_info = (AVOutputFormat* @ofmt, AVStream* @ost, AVStream* @ist, AVTimebaseSource @copy_tb) =>
        {
            vectors.avformat_transfer_internal_stream_timing_info = FunctionResolver.GetFunctionDelegate<vectors.avformat_transfer_internal_stream_timing_info_delegate>("avformat", "avformat_transfer_internal_stream_timing_info", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_transfer_internal_stream_timing_info(@ofmt, @ost, @ist, @copy_tb);
        };
        
        vectors.avformat_version = () =>
        {
            vectors.avformat_version = FunctionResolver.GetFunctionDelegate<vectors.avformat_version_delegate>("avformat", "avformat_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_version();
        };
        
        vectors.avformat_write_header = (AVFormatContext* @s, AVDictionary** @options) =>
        {
            vectors.avformat_write_header = FunctionResolver.GetFunctionDelegate<vectors.avformat_write_header_delegate>("avformat", "avformat_write_header", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avformat_write_header(@s, @options);
        };
        
        vectors.avio_accept = (AVIOContext* @s, AVIOContext** @c) =>
        {
            vectors.avio_accept = FunctionResolver.GetFunctionDelegate<vectors.avio_accept_delegate>("avformat", "avio_accept", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_accept(@s, @c);
        };
        
        vectors.avio_alloc_context = (byte* @buffer, int @buffer_size, int @write_flag, void* @opaque, avio_alloc_context_read_packet_func @read_packet, avio_alloc_context_write_packet_func @write_packet, avio_alloc_context_seek_func @seek) =>
        {
            vectors.avio_alloc_context = FunctionResolver.GetFunctionDelegate<vectors.avio_alloc_context_delegate>("avformat", "avio_alloc_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_alloc_context(@buffer, @buffer_size, @write_flag, @opaque, @read_packet, @write_packet, @seek);
        };
        
        vectors.avio_check = (string @url, int @flags) =>
        {
            vectors.avio_check = FunctionResolver.GetFunctionDelegate<vectors.avio_check_delegate>("avformat", "avio_check", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_check(@url, @flags);
        };
        
        vectors.avio_close = (AVIOContext* @s) =>
        {
            vectors.avio_close = FunctionResolver.GetFunctionDelegate<vectors.avio_close_delegate>("avformat", "avio_close", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_close(@s);
        };
        
        vectors.avio_close_dir = (AVIODirContext** @s) =>
        {
            vectors.avio_close_dir = FunctionResolver.GetFunctionDelegate<vectors.avio_close_dir_delegate>("avformat", "avio_close_dir", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_close_dir(@s);
        };
        
        vectors.avio_close_dyn_buf = (AVIOContext* @s, byte** @pbuffer) =>
        {
            vectors.avio_close_dyn_buf = FunctionResolver.GetFunctionDelegate<vectors.avio_close_dyn_buf_delegate>("avformat", "avio_close_dyn_buf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_close_dyn_buf(@s, @pbuffer);
        };
        
        vectors.avio_closep = (AVIOContext** @s) =>
        {
            vectors.avio_closep = FunctionResolver.GetFunctionDelegate<vectors.avio_closep_delegate>("avformat", "avio_closep", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_closep(@s);
        };
        
        vectors.avio_context_free = (AVIOContext** @s) =>
        {
            vectors.avio_context_free = FunctionResolver.GetFunctionDelegate<vectors.avio_context_free_delegate>("avformat", "avio_context_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_context_free(@s);
        };
        
        vectors.avio_enum_protocols = (void** @opaque, int @output) =>
        {
            vectors.avio_enum_protocols = FunctionResolver.GetFunctionDelegate<vectors.avio_enum_protocols_delegate>("avformat", "avio_enum_protocols", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_enum_protocols(@opaque, @output);
        };
        
        vectors.avio_feof = (AVIOContext* @s) =>
        {
            vectors.avio_feof = FunctionResolver.GetFunctionDelegate<vectors.avio_feof_delegate>("avformat", "avio_feof", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_feof(@s);
        };
        
        vectors.avio_find_protocol_name = (string @url) =>
        {
            vectors.avio_find_protocol_name = FunctionResolver.GetFunctionDelegate<vectors.avio_find_protocol_name_delegate>("avformat", "avio_find_protocol_name", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_find_protocol_name(@url);
        };
        
        vectors.avio_flush = (AVIOContext* @s) =>
        {
            vectors.avio_flush = FunctionResolver.GetFunctionDelegate<vectors.avio_flush_delegate>("avformat", "avio_flush", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_flush(@s);
        };
        
        vectors.avio_free_directory_entry = (AVIODirEntry** @entry) =>
        {
            vectors.avio_free_directory_entry = FunctionResolver.GetFunctionDelegate<vectors.avio_free_directory_entry_delegate>("avformat", "avio_free_directory_entry", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_free_directory_entry(@entry);
        };
        
        vectors.avio_get_dyn_buf = (AVIOContext* @s, byte** @pbuffer) =>
        {
            vectors.avio_get_dyn_buf = FunctionResolver.GetFunctionDelegate<vectors.avio_get_dyn_buf_delegate>("avformat", "avio_get_dyn_buf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_get_dyn_buf(@s, @pbuffer);
        };
        
        vectors.avio_get_str = (AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) =>
        {
            vectors.avio_get_str = FunctionResolver.GetFunctionDelegate<vectors.avio_get_str_delegate>("avformat", "avio_get_str", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_get_str(@pb, @maxlen, @buf, @buflen);
        };
        
        vectors.avio_get_str16be = (AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) =>
        {
            vectors.avio_get_str16be = FunctionResolver.GetFunctionDelegate<vectors.avio_get_str16be_delegate>("avformat", "avio_get_str16be", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_get_str16be(@pb, @maxlen, @buf, @buflen);
        };
        
        vectors.avio_get_str16le = (AVIOContext* @pb, int @maxlen, byte* @buf, int @buflen) =>
        {
            vectors.avio_get_str16le = FunctionResolver.GetFunctionDelegate<vectors.avio_get_str16le_delegate>("avformat", "avio_get_str16le", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_get_str16le(@pb, @maxlen, @buf, @buflen);
        };
        
        vectors.avio_handshake = (AVIOContext* @c) =>
        {
            vectors.avio_handshake = FunctionResolver.GetFunctionDelegate<vectors.avio_handshake_delegate>("avformat", "avio_handshake", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_handshake(@c);
        };
        
        vectors.avio_open = (AVIOContext** @s, string @url, int @flags) =>
        {
            vectors.avio_open = FunctionResolver.GetFunctionDelegate<vectors.avio_open_delegate>("avformat", "avio_open", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_open(@s, @url, @flags);
        };
        
        vectors.avio_open_dir = (AVIODirContext** @s, string @url, AVDictionary** @options) =>
        {
            vectors.avio_open_dir = FunctionResolver.GetFunctionDelegate<vectors.avio_open_dir_delegate>("avformat", "avio_open_dir", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_open_dir(@s, @url, @options);
        };
        
        vectors.avio_open_dyn_buf = (AVIOContext** @s) =>
        {
            vectors.avio_open_dyn_buf = FunctionResolver.GetFunctionDelegate<vectors.avio_open_dyn_buf_delegate>("avformat", "avio_open_dyn_buf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_open_dyn_buf(@s);
        };
        
        vectors.avio_open2 = (AVIOContext** @s, string @url, int @flags, AVIOInterruptCB* @int_cb, AVDictionary** @options) =>
        {
            vectors.avio_open2 = FunctionResolver.GetFunctionDelegate<vectors.avio_open2_delegate>("avformat", "avio_open2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_open2(@s, @url, @flags, @int_cb, @options);
        };
        
        vectors.avio_pause = (AVIOContext* @h, int @pause) =>
        {
            vectors.avio_pause = FunctionResolver.GetFunctionDelegate<vectors.avio_pause_delegate>("avformat", "avio_pause", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_pause(@h, @pause);
        };
        
        vectors.avio_print_string_array = (AVIOContext* @s, byte*[] @strings) =>
        {
            vectors.avio_print_string_array = FunctionResolver.GetFunctionDelegate<vectors.avio_print_string_array_delegate>("avformat", "avio_print_string_array", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_print_string_array(@s, @strings);
        };
        
        vectors.avio_printf = (AVIOContext* @s, string @fmt) =>
        {
            vectors.avio_printf = FunctionResolver.GetFunctionDelegate<vectors.avio_printf_delegate>("avformat", "avio_printf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_printf(@s, @fmt);
        };
        
        vectors.avio_protocol_get_class = (string @name) =>
        {
            vectors.avio_protocol_get_class = FunctionResolver.GetFunctionDelegate<vectors.avio_protocol_get_class_delegate>("avformat", "avio_protocol_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_protocol_get_class(@name);
        };
        
        vectors.avio_put_str = (AVIOContext* @s, string @str) =>
        {
            vectors.avio_put_str = FunctionResolver.GetFunctionDelegate<vectors.avio_put_str_delegate>("avformat", "avio_put_str", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_put_str(@s, @str);
        };
        
        vectors.avio_put_str16be = (AVIOContext* @s, string @str) =>
        {
            vectors.avio_put_str16be = FunctionResolver.GetFunctionDelegate<vectors.avio_put_str16be_delegate>("avformat", "avio_put_str16be", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_put_str16be(@s, @str);
        };
        
        vectors.avio_put_str16le = (AVIOContext* @s, string @str) =>
        {
            vectors.avio_put_str16le = FunctionResolver.GetFunctionDelegate<vectors.avio_put_str16le_delegate>("avformat", "avio_put_str16le", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_put_str16le(@s, @str);
        };
        
        vectors.avio_r8 = (AVIOContext* @s) =>
        {
            vectors.avio_r8 = FunctionResolver.GetFunctionDelegate<vectors.avio_r8_delegate>("avformat", "avio_r8", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_r8(@s);
        };
        
        vectors.avio_rb16 = (AVIOContext* @s) =>
        {
            vectors.avio_rb16 = FunctionResolver.GetFunctionDelegate<vectors.avio_rb16_delegate>("avformat", "avio_rb16", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rb16(@s);
        };
        
        vectors.avio_rb24 = (AVIOContext* @s) =>
        {
            vectors.avio_rb24 = FunctionResolver.GetFunctionDelegate<vectors.avio_rb24_delegate>("avformat", "avio_rb24", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rb24(@s);
        };
        
        vectors.avio_rb32 = (AVIOContext* @s) =>
        {
            vectors.avio_rb32 = FunctionResolver.GetFunctionDelegate<vectors.avio_rb32_delegate>("avformat", "avio_rb32", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rb32(@s);
        };
        
        vectors.avio_rb64 = (AVIOContext* @s) =>
        {
            vectors.avio_rb64 = FunctionResolver.GetFunctionDelegate<vectors.avio_rb64_delegate>("avformat", "avio_rb64", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rb64(@s);
        };
        
        vectors.avio_read = (AVIOContext* @s, byte* @buf, int @size) =>
        {
            vectors.avio_read = FunctionResolver.GetFunctionDelegate<vectors.avio_read_delegate>("avformat", "avio_read", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_read(@s, @buf, @size);
        };
        
        vectors.avio_read_dir = (AVIODirContext* @s, AVIODirEntry** @next) =>
        {
            vectors.avio_read_dir = FunctionResolver.GetFunctionDelegate<vectors.avio_read_dir_delegate>("avformat", "avio_read_dir", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_read_dir(@s, @next);
        };
        
        vectors.avio_read_partial = (AVIOContext* @s, byte* @buf, int @size) =>
        {
            vectors.avio_read_partial = FunctionResolver.GetFunctionDelegate<vectors.avio_read_partial_delegate>("avformat", "avio_read_partial", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_read_partial(@s, @buf, @size);
        };
        
        vectors.avio_read_to_bprint = (AVIOContext* @h, AVBPrint* @pb, ulong @max_size) =>
        {
            vectors.avio_read_to_bprint = FunctionResolver.GetFunctionDelegate<vectors.avio_read_to_bprint_delegate>("avformat", "avio_read_to_bprint", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_read_to_bprint(@h, @pb, @max_size);
        };
        
        vectors.avio_rl16 = (AVIOContext* @s) =>
        {
            vectors.avio_rl16 = FunctionResolver.GetFunctionDelegate<vectors.avio_rl16_delegate>("avformat", "avio_rl16", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rl16(@s);
        };
        
        vectors.avio_rl24 = (AVIOContext* @s) =>
        {
            vectors.avio_rl24 = FunctionResolver.GetFunctionDelegate<vectors.avio_rl24_delegate>("avformat", "avio_rl24", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rl24(@s);
        };
        
        vectors.avio_rl32 = (AVIOContext* @s) =>
        {
            vectors.avio_rl32 = FunctionResolver.GetFunctionDelegate<vectors.avio_rl32_delegate>("avformat", "avio_rl32", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rl32(@s);
        };
        
        vectors.avio_rl64 = (AVIOContext* @s) =>
        {
            vectors.avio_rl64 = FunctionResolver.GetFunctionDelegate<vectors.avio_rl64_delegate>("avformat", "avio_rl64", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_rl64(@s);
        };
        
        vectors.avio_seek = (AVIOContext* @s, long @offset, int @whence) =>
        {
            vectors.avio_seek = FunctionResolver.GetFunctionDelegate<vectors.avio_seek_delegate>("avformat", "avio_seek", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_seek(@s, @offset, @whence);
        };
        
        vectors.avio_seek_time = (AVIOContext* @h, int @stream_index, long @timestamp, int @flags) =>
        {
            vectors.avio_seek_time = FunctionResolver.GetFunctionDelegate<vectors.avio_seek_time_delegate>("avformat", "avio_seek_time", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_seek_time(@h, @stream_index, @timestamp, @flags);
        };
        
        vectors.avio_size = (AVIOContext* @s) =>
        {
            vectors.avio_size = FunctionResolver.GetFunctionDelegate<vectors.avio_size_delegate>("avformat", "avio_size", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_size(@s);
        };
        
        vectors.avio_skip = (AVIOContext* @s, long @offset) =>
        {
            vectors.avio_skip = FunctionResolver.GetFunctionDelegate<vectors.avio_skip_delegate>("avformat", "avio_skip", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_skip(@s, @offset);
        };
        
        vectors.avio_vprintf = (AVIOContext* @s, string @fmt, byte* @ap) =>
        {
            vectors.avio_vprintf = FunctionResolver.GetFunctionDelegate<vectors.avio_vprintf_delegate>("avformat", "avio_vprintf", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avio_vprintf(@s, @fmt, @ap);
        };
        
        vectors.avio_w8 = (AVIOContext* @s, int @b) =>
        {
            vectors.avio_w8 = FunctionResolver.GetFunctionDelegate<vectors.avio_w8_delegate>("avformat", "avio_w8", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_w8(@s, @b);
        };
        
        vectors.avio_wb16 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wb16 = FunctionResolver.GetFunctionDelegate<vectors.avio_wb16_delegate>("avformat", "avio_wb16", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wb16(@s, @val);
        };
        
        vectors.avio_wb24 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wb24 = FunctionResolver.GetFunctionDelegate<vectors.avio_wb24_delegate>("avformat", "avio_wb24", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wb24(@s, @val);
        };
        
        vectors.avio_wb32 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wb32 = FunctionResolver.GetFunctionDelegate<vectors.avio_wb32_delegate>("avformat", "avio_wb32", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wb32(@s, @val);
        };
        
        vectors.avio_wb64 = (AVIOContext* @s, ulong @val) =>
        {
            vectors.avio_wb64 = FunctionResolver.GetFunctionDelegate<vectors.avio_wb64_delegate>("avformat", "avio_wb64", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wb64(@s, @val);
        };
        
        vectors.avio_wl16 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wl16 = FunctionResolver.GetFunctionDelegate<vectors.avio_wl16_delegate>("avformat", "avio_wl16", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wl16(@s, @val);
        };
        
        vectors.avio_wl24 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wl24 = FunctionResolver.GetFunctionDelegate<vectors.avio_wl24_delegate>("avformat", "avio_wl24", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wl24(@s, @val);
        };
        
        vectors.avio_wl32 = (AVIOContext* @s, uint @val) =>
        {
            vectors.avio_wl32 = FunctionResolver.GetFunctionDelegate<vectors.avio_wl32_delegate>("avformat", "avio_wl32", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wl32(@s, @val);
        };
        
        vectors.avio_wl64 = (AVIOContext* @s, ulong @val) =>
        {
            vectors.avio_wl64 = FunctionResolver.GetFunctionDelegate<vectors.avio_wl64_delegate>("avformat", "avio_wl64", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_wl64(@s, @val);
        };
        
        vectors.avio_write = (AVIOContext* @s, byte* @buf, int @size) =>
        {
            vectors.avio_write = FunctionResolver.GetFunctionDelegate<vectors.avio_write_delegate>("avformat", "avio_write", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_write(@s, @buf, @size);
        };
        
        vectors.avio_write_marker = (AVIOContext* @s, long @time, AVIODataMarkerType @type) =>
        {
            vectors.avio_write_marker = FunctionResolver.GetFunctionDelegate<vectors.avio_write_marker_delegate>("avformat", "avio_write_marker", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avio_write_marker(@s, @time, @type);
        };
        
        vectors.avsubtitle_free = (AVSubtitle* @sub) =>
        {
            vectors.avsubtitle_free = FunctionResolver.GetFunctionDelegate<vectors.avsubtitle_free_delegate>("avcodec", "avsubtitle_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.avsubtitle_free(@sub);
        };
        
        vectors.avutil_configuration = () =>
        {
            vectors.avutil_configuration = FunctionResolver.GetFunctionDelegate<vectors.avutil_configuration_delegate>("avutil", "avutil_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avutil_configuration();
        };
        
        vectors.avutil_license = () =>
        {
            vectors.avutil_license = FunctionResolver.GetFunctionDelegate<vectors.avutil_license_delegate>("avutil", "avutil_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avutil_license();
        };
        
        vectors.avutil_version = () =>
        {
            vectors.avutil_version = FunctionResolver.GetFunctionDelegate<vectors.avutil_version_delegate>("avutil", "avutil_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.avutil_version();
        };
        
        vectors.postproc_configuration = () =>
        {
            vectors.postproc_configuration = FunctionResolver.GetFunctionDelegate<vectors.postproc_configuration_delegate>("postproc", "postproc_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.postproc_configuration();
        };
        
        vectors.postproc_license = () =>
        {
            vectors.postproc_license = FunctionResolver.GetFunctionDelegate<vectors.postproc_license_delegate>("postproc", "postproc_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.postproc_license();
        };
        
        vectors.postproc_version = () =>
        {
            vectors.postproc_version = FunctionResolver.GetFunctionDelegate<vectors.postproc_version_delegate>("postproc", "postproc_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.postproc_version();
        };
        
        vectors.pp_free_context = (void* @ppContext) =>
        {
            vectors.pp_free_context = FunctionResolver.GetFunctionDelegate<vectors.pp_free_context_delegate>("postproc", "pp_free_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.pp_free_context(@ppContext);
        };
        
        vectors.pp_free_mode = (void* @mode) =>
        {
            vectors.pp_free_mode = FunctionResolver.GetFunctionDelegate<vectors.pp_free_mode_delegate>("postproc", "pp_free_mode", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.pp_free_mode(@mode);
        };
        
        vectors.pp_get_context = (int @width, int @height, int @flags) =>
        {
            vectors.pp_get_context = FunctionResolver.GetFunctionDelegate<vectors.pp_get_context_delegate>("postproc", "pp_get_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.pp_get_context(@width, @height, @flags);
        };
        
        vectors.pp_get_mode_by_name_and_quality = (string @name, int @quality) =>
        {
            vectors.pp_get_mode_by_name_and_quality = FunctionResolver.GetFunctionDelegate<vectors.pp_get_mode_by_name_and_quality_delegate>("postproc", "pp_get_mode_by_name_and_quality", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.pp_get_mode_by_name_and_quality(@name, @quality);
        };
        
        vectors.pp_postprocess = (in byte_ptrArray3 @src, in int_array3 @srcStride, ref byte_ptrArray3 @dst, in int_array3 @dstStride, int @horizontalSize, int @verticalSize, sbyte* @QP_store, int @QP_stride, void* @mode, void* @ppContext, int @pict_type) =>
        {
            vectors.pp_postprocess = FunctionResolver.GetFunctionDelegate<vectors.pp_postprocess_delegate>("postproc", "pp_postprocess", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.pp_postprocess(@src, @srcStride, ref @dst, @dstStride, @horizontalSize, @verticalSize, @QP_store, @QP_stride, @mode, @ppContext, @pict_type);
        };
        
        vectors.swr_alloc = () =>
        {
            vectors.swr_alloc = FunctionResolver.GetFunctionDelegate<vectors.swr_alloc_delegate>("swresample", "swr_alloc", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_alloc();
        };
        
        vectors.swr_alloc_set_opts = (SwrContext* @s, long @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, long @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx) =>
        {
            vectors.swr_alloc_set_opts = FunctionResolver.GetFunctionDelegate<vectors.swr_alloc_set_opts_delegate>("swresample", "swr_alloc_set_opts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_alloc_set_opts(@s, @out_ch_layout, @out_sample_fmt, @out_sample_rate, @in_ch_layout, @in_sample_fmt, @in_sample_rate, @log_offset, @log_ctx);
        };
        
        vectors.swr_alloc_set_opts2 = (SwrContext** @ps, AVChannelLayout* @out_ch_layout, AVSampleFormat @out_sample_fmt, int @out_sample_rate, AVChannelLayout* @in_ch_layout, AVSampleFormat @in_sample_fmt, int @in_sample_rate, int @log_offset, void* @log_ctx) =>
        {
            vectors.swr_alloc_set_opts2 = FunctionResolver.GetFunctionDelegate<vectors.swr_alloc_set_opts2_delegate>("swresample", "swr_alloc_set_opts2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_alloc_set_opts2(@ps, @out_ch_layout, @out_sample_fmt, @out_sample_rate, @in_ch_layout, @in_sample_fmt, @in_sample_rate, @log_offset, @log_ctx);
        };
        
        vectors.swr_build_matrix = (ulong @in_layout, ulong @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @rematrix_maxval, double @rematrix_volume, double* @matrix, int @stride, AVMatrixEncoding @matrix_encoding, void* @log_ctx) =>
        {
            vectors.swr_build_matrix = FunctionResolver.GetFunctionDelegate<vectors.swr_build_matrix_delegate>("swresample", "swr_build_matrix", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_build_matrix(@in_layout, @out_layout, @center_mix_level, @surround_mix_level, @lfe_mix_level, @rematrix_maxval, @rematrix_volume, @matrix, @stride, @matrix_encoding, @log_ctx);
        };
        
        vectors.swr_build_matrix2 = (AVChannelLayout* @in_layout, AVChannelLayout* @out_layout, double @center_mix_level, double @surround_mix_level, double @lfe_mix_level, double @maxval, double @rematrix_volume, double* @matrix, long @stride, AVMatrixEncoding @matrix_encoding, void* @log_context) =>
        {
            vectors.swr_build_matrix2 = FunctionResolver.GetFunctionDelegate<vectors.swr_build_matrix2_delegate>("swresample", "swr_build_matrix2", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_build_matrix2(@in_layout, @out_layout, @center_mix_level, @surround_mix_level, @lfe_mix_level, @maxval, @rematrix_volume, @matrix, @stride, @matrix_encoding, @log_context);
        };
        
        vectors.swr_close = (SwrContext* @s) =>
        {
            vectors.swr_close = FunctionResolver.GetFunctionDelegate<vectors.swr_close_delegate>("swresample", "swr_close", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.swr_close(@s);
        };
        
        vectors.swr_config_frame = (SwrContext* @swr, AVFrame* @out, AVFrame* @in) =>
        {
            vectors.swr_config_frame = FunctionResolver.GetFunctionDelegate<vectors.swr_config_frame_delegate>("swresample", "swr_config_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_config_frame(@swr, @out, @in);
        };
        
        vectors.swr_convert = (SwrContext* @s, byte** @out, int @out_count, byte** @in, int @in_count) =>
        {
            vectors.swr_convert = FunctionResolver.GetFunctionDelegate<vectors.swr_convert_delegate>("swresample", "swr_convert", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_convert(@s, @out, @out_count, @in, @in_count);
        };
        
        vectors.swr_convert_frame = (SwrContext* @swr, AVFrame* @output, AVFrame* @input) =>
        {
            vectors.swr_convert_frame = FunctionResolver.GetFunctionDelegate<vectors.swr_convert_frame_delegate>("swresample", "swr_convert_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_convert_frame(@swr, @output, @input);
        };
        
        vectors.swr_drop_output = (SwrContext* @s, int @count) =>
        {
            vectors.swr_drop_output = FunctionResolver.GetFunctionDelegate<vectors.swr_drop_output_delegate>("swresample", "swr_drop_output", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_drop_output(@s, @count);
        };
        
        vectors.swr_free = (SwrContext** @s) =>
        {
            vectors.swr_free = FunctionResolver.GetFunctionDelegate<vectors.swr_free_delegate>("swresample", "swr_free", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.swr_free(@s);
        };
        
        vectors.swr_get_class = () =>
        {
            vectors.swr_get_class = FunctionResolver.GetFunctionDelegate<vectors.swr_get_class_delegate>("swresample", "swr_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_get_class();
        };
        
        vectors.swr_get_delay = (SwrContext* @s, long @base) =>
        {
            vectors.swr_get_delay = FunctionResolver.GetFunctionDelegate<vectors.swr_get_delay_delegate>("swresample", "swr_get_delay", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_get_delay(@s, @base);
        };
        
        vectors.swr_get_out_samples = (SwrContext* @s, int @in_samples) =>
        {
            vectors.swr_get_out_samples = FunctionResolver.GetFunctionDelegate<vectors.swr_get_out_samples_delegate>("swresample", "swr_get_out_samples", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_get_out_samples(@s, @in_samples);
        };
        
        vectors.swr_init = (SwrContext* @s) =>
        {
            vectors.swr_init = FunctionResolver.GetFunctionDelegate<vectors.swr_init_delegate>("swresample", "swr_init", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_init(@s);
        };
        
        vectors.swr_inject_silence = (SwrContext* @s, int @count) =>
        {
            vectors.swr_inject_silence = FunctionResolver.GetFunctionDelegate<vectors.swr_inject_silence_delegate>("swresample", "swr_inject_silence", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_inject_silence(@s, @count);
        };
        
        vectors.swr_is_initialized = (SwrContext* @s) =>
        {
            vectors.swr_is_initialized = FunctionResolver.GetFunctionDelegate<vectors.swr_is_initialized_delegate>("swresample", "swr_is_initialized", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_is_initialized(@s);
        };
        
        vectors.swr_next_pts = (SwrContext* @s, long @pts) =>
        {
            vectors.swr_next_pts = FunctionResolver.GetFunctionDelegate<vectors.swr_next_pts_delegate>("swresample", "swr_next_pts", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_next_pts(@s, @pts);
        };
        
        vectors.swr_set_channel_mapping = (SwrContext* @s, int* @channel_map) =>
        {
            vectors.swr_set_channel_mapping = FunctionResolver.GetFunctionDelegate<vectors.swr_set_channel_mapping_delegate>("swresample", "swr_set_channel_mapping", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_set_channel_mapping(@s, @channel_map);
        };
        
        vectors.swr_set_compensation = (SwrContext* @s, int @sample_delta, int @compensation_distance) =>
        {
            vectors.swr_set_compensation = FunctionResolver.GetFunctionDelegate<vectors.swr_set_compensation_delegate>("swresample", "swr_set_compensation", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_set_compensation(@s, @sample_delta, @compensation_distance);
        };
        
        vectors.swr_set_matrix = (SwrContext* @s, double* @matrix, int @stride) =>
        {
            vectors.swr_set_matrix = FunctionResolver.GetFunctionDelegate<vectors.swr_set_matrix_delegate>("swresample", "swr_set_matrix", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swr_set_matrix(@s, @matrix, @stride);
        };
        
        vectors.swresample_configuration = () =>
        {
            vectors.swresample_configuration = FunctionResolver.GetFunctionDelegate<vectors.swresample_configuration_delegate>("swresample", "swresample_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swresample_configuration();
        };
        
        vectors.swresample_license = () =>
        {
            vectors.swresample_license = FunctionResolver.GetFunctionDelegate<vectors.swresample_license_delegate>("swresample", "swresample_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swresample_license();
        };
        
        vectors.swresample_version = () =>
        {
            vectors.swresample_version = FunctionResolver.GetFunctionDelegate<vectors.swresample_version_delegate>("swresample", "swresample_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swresample_version();
        };
        
        vectors.sws_alloc_context = () =>
        {
            vectors.sws_alloc_context = FunctionResolver.GetFunctionDelegate<vectors.sws_alloc_context_delegate>("swscale", "sws_alloc_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_alloc_context();
        };
        
        vectors.sws_allocVec = (int @length) =>
        {
            vectors.sws_allocVec = FunctionResolver.GetFunctionDelegate<vectors.sws_allocVec_delegate>("swscale", "sws_allocVec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_allocVec(@length);
        };
        
        vectors.sws_convertPalette8ToPacked24 = (byte* @src, byte* @dst, int @num_pixels, byte* @palette) =>
        {
            vectors.sws_convertPalette8ToPacked24 = FunctionResolver.GetFunctionDelegate<vectors.sws_convertPalette8ToPacked24_delegate>("swscale", "sws_convertPalette8ToPacked24", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_convertPalette8ToPacked24(@src, @dst, @num_pixels, @palette);
        };
        
        vectors.sws_convertPalette8ToPacked32 = (byte* @src, byte* @dst, int @num_pixels, byte* @palette) =>
        {
            vectors.sws_convertPalette8ToPacked32 = FunctionResolver.GetFunctionDelegate<vectors.sws_convertPalette8ToPacked32_delegate>("swscale", "sws_convertPalette8ToPacked32", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_convertPalette8ToPacked32(@src, @dst, @num_pixels, @palette);
        };
        
        vectors.sws_frame_end = (SwsContext* @c) =>
        {
            vectors.sws_frame_end = FunctionResolver.GetFunctionDelegate<vectors.sws_frame_end_delegate>("swscale", "sws_frame_end", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_frame_end(@c);
        };
        
        vectors.sws_frame_start = (SwsContext* @c, AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.sws_frame_start = FunctionResolver.GetFunctionDelegate<vectors.sws_frame_start_delegate>("swscale", "sws_frame_start", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_frame_start(@c, @dst, @src);
        };
        
        vectors.sws_freeContext = (SwsContext* @swsContext) =>
        {
            vectors.sws_freeContext = FunctionResolver.GetFunctionDelegate<vectors.sws_freeContext_delegate>("swscale", "sws_freeContext", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_freeContext(@swsContext);
        };
        
        vectors.sws_freeFilter = (SwsFilter* @filter) =>
        {
            vectors.sws_freeFilter = FunctionResolver.GetFunctionDelegate<vectors.sws_freeFilter_delegate>("swscale", "sws_freeFilter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_freeFilter(@filter);
        };
        
        vectors.sws_freeVec = (SwsVector* @a) =>
        {
            vectors.sws_freeVec = FunctionResolver.GetFunctionDelegate<vectors.sws_freeVec_delegate>("swscale", "sws_freeVec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_freeVec(@a);
        };
        
        vectors.sws_get_class = () =>
        {
            vectors.sws_get_class = FunctionResolver.GetFunctionDelegate<vectors.sws_get_class_delegate>("swscale", "sws_get_class", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_get_class();
        };
        
        vectors.sws_getCachedContext = (SwsContext* @context, int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param) =>
        {
            vectors.sws_getCachedContext = FunctionResolver.GetFunctionDelegate<vectors.sws_getCachedContext_delegate>("swscale", "sws_getCachedContext", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getCachedContext(@context, @srcW, @srcH, @srcFormat, @dstW, @dstH, @dstFormat, @flags, @srcFilter, @dstFilter, @param);
        };
        
        vectors.sws_getCoefficients = (int @colorspace) =>
        {
            vectors.sws_getCoefficients = FunctionResolver.GetFunctionDelegate<vectors.sws_getCoefficients_delegate>("swscale", "sws_getCoefficients", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getCoefficients(@colorspace);
        };
        
        vectors.sws_getColorspaceDetails = (SwsContext* @c, int** @inv_table, int* @srcRange, int** @table, int* @dstRange, int* @brightness, int* @contrast, int* @saturation) =>
        {
            vectors.sws_getColorspaceDetails = FunctionResolver.GetFunctionDelegate<vectors.sws_getColorspaceDetails_delegate>("swscale", "sws_getColorspaceDetails", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getColorspaceDetails(@c, @inv_table, @srcRange, @table, @dstRange, @brightness, @contrast, @saturation);
        };
        
        vectors.sws_getContext = (int @srcW, int @srcH, AVPixelFormat @srcFormat, int @dstW, int @dstH, AVPixelFormat @dstFormat, int @flags, SwsFilter* @srcFilter, SwsFilter* @dstFilter, double* @param) =>
        {
            vectors.sws_getContext = FunctionResolver.GetFunctionDelegate<vectors.sws_getContext_delegate>("swscale", "sws_getContext", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getContext(@srcW, @srcH, @srcFormat, @dstW, @dstH, @dstFormat, @flags, @srcFilter, @dstFilter, @param);
        };
        
        vectors.sws_getDefaultFilter = (float @lumaGBlur, float @chromaGBlur, float @lumaSharpen, float @chromaSharpen, float @chromaHShift, float @chromaVShift, int @verbose) =>
        {
            vectors.sws_getDefaultFilter = FunctionResolver.GetFunctionDelegate<vectors.sws_getDefaultFilter_delegate>("swscale", "sws_getDefaultFilter", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getDefaultFilter(@lumaGBlur, @chromaGBlur, @lumaSharpen, @chromaSharpen, @chromaHShift, @chromaVShift, @verbose);
        };
        
        vectors.sws_getGaussianVec = (double @variance, double @quality) =>
        {
            vectors.sws_getGaussianVec = FunctionResolver.GetFunctionDelegate<vectors.sws_getGaussianVec_delegate>("swscale", "sws_getGaussianVec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_getGaussianVec(@variance, @quality);
        };
        
        vectors.sws_init_context = (SwsContext* @sws_context, SwsFilter* @srcFilter, SwsFilter* @dstFilter) =>
        {
            vectors.sws_init_context = FunctionResolver.GetFunctionDelegate<vectors.sws_init_context_delegate>("swscale", "sws_init_context", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_init_context(@sws_context, @srcFilter, @dstFilter);
        };
        
        vectors.sws_isSupportedEndiannessConversion = (AVPixelFormat @pix_fmt) =>
        {
            vectors.sws_isSupportedEndiannessConversion = FunctionResolver.GetFunctionDelegate<vectors.sws_isSupportedEndiannessConversion_delegate>("swscale", "sws_isSupportedEndiannessConversion", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_isSupportedEndiannessConversion(@pix_fmt);
        };
        
        vectors.sws_isSupportedInput = (AVPixelFormat @pix_fmt) =>
        {
            vectors.sws_isSupportedInput = FunctionResolver.GetFunctionDelegate<vectors.sws_isSupportedInput_delegate>("swscale", "sws_isSupportedInput", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_isSupportedInput(@pix_fmt);
        };
        
        vectors.sws_isSupportedOutput = (AVPixelFormat @pix_fmt) =>
        {
            vectors.sws_isSupportedOutput = FunctionResolver.GetFunctionDelegate<vectors.sws_isSupportedOutput_delegate>("swscale", "sws_isSupportedOutput", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_isSupportedOutput(@pix_fmt);
        };
        
        vectors.sws_normalizeVec = (SwsVector* @a, double @height) =>
        {
            vectors.sws_normalizeVec = FunctionResolver.GetFunctionDelegate<vectors.sws_normalizeVec_delegate>("swscale", "sws_normalizeVec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_normalizeVec(@a, @height);
        };
        
        vectors.sws_receive_slice = (SwsContext* @c, uint @slice_start, uint @slice_height) =>
        {
            vectors.sws_receive_slice = FunctionResolver.GetFunctionDelegate<vectors.sws_receive_slice_delegate>("swscale", "sws_receive_slice", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_receive_slice(@c, @slice_start, @slice_height);
        };
        
        vectors.sws_receive_slice_alignment = (SwsContext* @c) =>
        {
            vectors.sws_receive_slice_alignment = FunctionResolver.GetFunctionDelegate<vectors.sws_receive_slice_alignment_delegate>("swscale", "sws_receive_slice_alignment", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_receive_slice_alignment(@c);
        };
        
        vectors.sws_scale = (SwsContext* @c, byte*[] @srcSlice, int[] @srcStride, int @srcSliceY, int @srcSliceH, byte*[] @dst, int[] @dstStride) =>
        {
            vectors.sws_scale = FunctionResolver.GetFunctionDelegate<vectors.sws_scale_delegate>("swscale", "sws_scale", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_scale(@c, @srcSlice, @srcStride, @srcSliceY, @srcSliceH, @dst, @dstStride);
        };
        
        vectors.sws_scale_frame = (SwsContext* @c, AVFrame* @dst, AVFrame* @src) =>
        {
            vectors.sws_scale_frame = FunctionResolver.GetFunctionDelegate<vectors.sws_scale_frame_delegate>("swscale", "sws_scale_frame", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_scale_frame(@c, @dst, @src);
        };
        
        vectors.sws_scaleVec = (SwsVector* @a, double @scalar) =>
        {
            vectors.sws_scaleVec = FunctionResolver.GetFunctionDelegate<vectors.sws_scaleVec_delegate>("swscale", "sws_scaleVec", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            vectors.sws_scaleVec(@a, @scalar);
        };
        
        vectors.sws_send_slice = (SwsContext* @c, uint @slice_start, uint @slice_height) =>
        {
            vectors.sws_send_slice = FunctionResolver.GetFunctionDelegate<vectors.sws_send_slice_delegate>("swscale", "sws_send_slice", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_send_slice(@c, @slice_start, @slice_height);
        };
        
        vectors.sws_setColorspaceDetails = (SwsContext* @c, in int_array4 @inv_table, int @srcRange, in int_array4 @table, int @dstRange, int @brightness, int @contrast, int @saturation) =>
        {
            vectors.sws_setColorspaceDetails = FunctionResolver.GetFunctionDelegate<vectors.sws_setColorspaceDetails_delegate>("swscale", "sws_setColorspaceDetails", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.sws_setColorspaceDetails(@c, @inv_table, @srcRange, @table, @dstRange, @brightness, @contrast, @saturation);
        };
        
        vectors.swscale_configuration = () =>
        {
            vectors.swscale_configuration = FunctionResolver.GetFunctionDelegate<vectors.swscale_configuration_delegate>("swscale", "swscale_configuration", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swscale_configuration();
        };
        
        vectors.swscale_license = () =>
        {
            vectors.swscale_license = FunctionResolver.GetFunctionDelegate<vectors.swscale_license_delegate>("swscale", "swscale_license", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swscale_license();
        };
        
        vectors.swscale_version = () =>
        {
            vectors.swscale_version = FunctionResolver.GetFunctionDelegate<vectors.swscale_version_delegate>("swscale", "swscale_version", ThrowErrorIfFunctionNotFound) ?? delegate { throw new NotSupportedException(); };
            return vectors.swscale_version();
        };
        
    }
}
